using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using static Database.ApiRequests.QueryCurrentUser;

namespace Database.Authorization;

public sealed class CommonAuthorization(
    UserService userService
)
{
    public Task<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        return userService.GetCurrentUser(cancellationToken);
    }

    public async Task<bool> IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(CancellationToken cancellationToken)
    {
        var currentUser = await GetCurrentUserAsync(cancellationToken);
        if (currentUser is null)
        {
            return false;
        }
        return IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser);
    }

    public bool IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(
        CurrentUser currentUser
    )
    {
        return currentUser.DatabaseOperatingRepresentedInstitutions.TotalCount >= 1;
    }

    public bool IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(
        CurrentUser currentUser,
        Guid institutionId
    )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t =>
            (
                t.Role is InstitutionRepresentativeRole.ASSISTANT
                || t.Role is InstitutionRepresentativeRole.OWNER
            )
            &&
            (
                t.Node.Uuid == institutionId
                || t.Node.ManagedInstitutions.Edges.Any(t =>
                    t.Node.Uuid == institutionId
                )
            )
        );
    }
}