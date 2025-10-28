using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Database.Enumerations;

namespace Database.GraphQl.AccessRights;

public sealed record UpdateDataAccessRightsInput
(
    Guid DataId,
    DataKind DataKind,
    IReadOnlyDictionary<Guid, uint?>? AllowedUserAndQuantity,
    IReadOnlyList<Guid>? AllowedInstitutions,
    IReadOnlyList<string>? AllowedApplications
);

[SuppressMessage("Naming", "CA1707")]
public enum UpdateDataAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record UpdateDataAccessRightsError(
    UpdateDataAccessRightsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateDataAccessRightsErrorCode>(Code, Message, Path);

public sealed class UpdateDataAccessRightsPayload
    : DataAccessRightsPayload<UpdateDataAccessRightsError>
{
    public UpdateDataAccessRightsPayload(
        DataAccessRights dataAccessRights
    )
        : base(dataAccessRights)
    {
    }

    public UpdateDataAccessRightsPayload(
        UpdateDataAccessRightsError error
    )
        : base(error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateDataAccessRightsMutation
{
    public async Task<UpdateDataAccessRightsPayload> UpdateDataAccessRightsAsync(
        UpdateDataAccessRightsInput input,
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update access rights for the data.",
                    []
                )
            );
        }

        var data = await context.GetDataAsync(input.DataId, input.DataKind, cancellationToken);
        if (data is null)
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    []
                )
            );
        }

        data.DataAccessRights.AllowedInstitutions = input.AllowedInstitutions;
        data.DataAccessRights.AllowedApplications = input.AllowedApplications;
        data.DataAccessRights.AllowedUserAndQuantity = input.AllowedUserAndQuantity;

        await context.SaveChangesAsync(cancellationToken);
        return new UpdateDataAccessRightsPayload(data.DataAccessRights);
    }
}