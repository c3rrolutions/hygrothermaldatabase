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

public sealed class UpdateInstitutionAccessRightsPayload
    : InstitutionAccessRightsPayload<UpdateInstitutionAccessRightsError>
{
    public UpdateInstitutionAccessRightsPayload(
        Data.InstitutionAccessRights accessRights
    )
        : base(accessRights)
    {
    }

    public UpdateInstitutionAccessRightsPayload(
        UpdateInstitutionAccessRightsError error
    )
        : base(error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateInstitutionAccessRightsMutation
{
    public async Task<UpdateInstitutionAccessRightsPayload> UpdateInstitutionAccessRightsAsync(
        UpdateInstitutionAccessRightsInput input,
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken);
        if (currentUser is null)
        {
            return new UpdateInstitutionAccessRightsPayload(
                new UpdateInstitutionAccessRightsError(
                    UpdateInstitutionAccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new UpdateInstitutionAccessRightsPayload(
                new UpdateInstitutionAccessRightsError(
                    UpdateInstitutionAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.InstitutionAccessRights.SingleOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken);

        if (accessRights is null)
        {
            return new UpdateInstitutionAccessRightsPayload(
                new UpdateInstitutionAccessRightsError(
                    UpdateInstitutionAccessRightsErrorCode.UNKNOWN_ACCESS_RIGHTS,
                    $"There are no access rights for this institution.",
                    []
                )
            );
        }

        accessRights.AllowedUserCount = input.AllowedUserCount;
        accessRights.AllowedDatasetsPerTime = input.AllowedDatasetsPerTimeSpan;
        accessRights.Period = TimeSpan.FromDays(input.PeriodInDays);
        await context.SaveChangesAsync(cancellationToken);
        return new UpdateInstitutionAccessRightsPayload(accessRights);
    }
}