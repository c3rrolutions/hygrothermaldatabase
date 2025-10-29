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

namespace Database.GraphQl.AccessRights;

public sealed record AddInstitutionAccessRightsInput
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
public enum AddInstitutionAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    ALREADY_EXISTS,
}

public sealed record AddInstitutionAccessRightsError(
    AddInstitutionAccessRightsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<AddInstitutionAccessRightsErrorCode>(Code, Message, Path);

public sealed record AddInstitutionAccessRightsPayload(
   Data.InstitutionAccessRights? InstitutionAccessRights,
   IReadOnlyCollection<AddInstitutionAccessRightsError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class AddInstitutionAccessRightsMutation
: MutationBase<Data.InstitutionAccessRights, AddInstitutionAccessRightsPayload, AddInstitutionAccessRightsError, AddInstitutionAccessRightsErrorCode>
{
    protected override AddInstitutionAccessRightsPayload NewPayload(
        Data.InstitutionAccessRights? data,
        IReadOnlyCollection<AddInstitutionAccessRightsError>? errors
    ) => new(data, errors);

    protected override AddInstitutionAccessRightsError NewError(
        AddInstitutionAccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<AddInstitutionAccessRightsPayload> AddInstitutionAccessRightsAsync(
        AddInstitutionAccessRightsInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                AddInstitutionAccessRightsErrorCode.UNAUTHENTICATED,
                AddInstitutionAccessRightsErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var accessRights = await context.InstitutionAccessRights
            .SingleOrDefaultAsync(x =>
                x.InstitutionId == input.InstitutionId,
                cancellationToken
            );
        if (accessRights is not null)
        {
            return NewPayload(
                null,
                [NewError(
                    AddInstitutionAccessRightsErrorCode.ALREADY_EXISTS,
                    $"The access rights for this institution already exist.",
                    []
                )]
            );
        }

        accessRights = new Data.InstitutionAccessRights(
            input.InstitutionId,
            input.AllowedUserCount,
            input.AllowedDatasetsPerTimeSpan,
            TimeSpan.FromDays(input.PeriodInDays));
        context.InstitutionAccessRights.Add(accessRights);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessRights, null);
    }
}