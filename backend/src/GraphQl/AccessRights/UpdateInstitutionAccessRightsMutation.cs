using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Database.Extensions;
using NodaTime;

namespace Database.GraphQl.AccessRights;

public sealed record UpdateInstitutionAccessRightsInput
(
    // Id of institution
    Guid InstitutionId,
    // Count of allowed user for institution. Null is unlimited
    uint? AllowedUserCount,
    // Count of allowed datasets for institution. Null is unlimited
    uint? AllowedDatasetsPerTimeSpan,
    int PeriodInDays
);

[SuppressMessage("Naming", "CA1707")]
public enum UpdateInstitutionAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_ACCESS_RIGHTS
}

public sealed record UpdateInstitutionAccessRightsError(
    UpdateInstitutionAccessRightsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateInstitutionAccessRightsErrorCode>(Code, Message, Path);

public sealed record UpdateInstitutionAccessRightsPayload(
    Data.InstitutionAccessRights? InstitutionAccessRights,
    IReadOnlyCollection<UpdateInstitutionAccessRightsError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateInstitutionAccessRightsMutation
: MutationBase<Data.InstitutionAccessRights, UpdateInstitutionAccessRightsPayload, UpdateInstitutionAccessRightsError, UpdateInstitutionAccessRightsErrorCode>
{
    protected override UpdateInstitutionAccessRightsPayload NewPayload(
        Data.InstitutionAccessRights? data,
        IReadOnlyCollection<UpdateInstitutionAccessRightsError>? errors
    ) => new(data, errors);

    protected override UpdateInstitutionAccessRightsError NewError(
        UpdateInstitutionAccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<UpdateInstitutionAccessRightsPayload> UpdateInstitutionAccessRightsAsync(
        UpdateInstitutionAccessRightsInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UpdateInstitutionAccessRightsErrorCode.UNAUTHENTICATED,
                UpdateInstitutionAccessRightsErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var accessRights = await context.InstitutionAccessRights
            .SingleOrDefaultAsync(x =>
                x.InstitutionId == input.InstitutionId,
                cancellationToken
            );
        if (accessRights is null)
        {
            return NewPayload(
                null,
                [NewError(
                    UpdateInstitutionAccessRightsErrorCode.UNKNOWN_ACCESS_RIGHTS,
                    $"There are no access rights for this institution.",
                    []
                )]
            );
        }

        accessRights.AllowedUserCount = input.AllowedUserCount;
        accessRights.AllowedDatasetsPerTime = input.AllowedDatasetsPerTimeSpan;
        accessRights.Period = Duration.FromDays(input.PeriodInDays);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessRights, null);
    }
}