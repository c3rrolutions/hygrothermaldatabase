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

public sealed class AddInstitutionAccessRightsPayload
    : InstitutionAccessRightsPayload<AddInstitutionAccessRightsError>
{
    public AddInstitutionAccessRightsPayload(
        Data.InstitutionAccessRights accessRights
    )
        : base(accessRights)
    {
    }

    public AddInstitutionAccessRightsPayload(
        AddInstitutionAccessRightsError error
    )
        : base(error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class AddInstitutionAccessRightsMutation
{
    public async Task<AddInstitutionAccessRightsPayload> AddInstitutionAccessRightsAsync(
        AddInstitutionAccessRightsInput input,
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
            return new AddInstitutionAccessRightsPayload(
                new AddInstitutionAccessRightsError(
                    AddInstitutionAccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new AddInstitutionAccessRightsPayload(
                new AddInstitutionAccessRightsError(
                    AddInstitutionAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.InstitutionAccessRights.SingleOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken);

        if (accessRights is not null)
        {
            return new AddInstitutionAccessRightsPayload(
                new AddInstitutionAccessRightsError(
                    AddInstitutionAccessRightsErrorCode.ALREADY_EXISTS,
                    $"The access rights for this institution already exist.",
                    []
                )
            );
        }

        accessRights = new Data.InstitutionAccessRights(
            input.InstitutionId,
            input.AllowedUserCount,
            input.AllowedDatasetsPerTimeSpan,
            TimeSpan.FromDays(input.PeriodInDays));
        context.InstitutionAccessRights.Add(accessRights);
        await context.SaveChangesAsync(cancellationToken);
        return new AddInstitutionAccessRightsPayload(accessRights);
    }
}