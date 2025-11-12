using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using static Database.ApiRequests.QueryCurrentUserOrApplication;

namespace Database.Authorization;

public sealed class CommonAuthorization(
    UserService userService
)
{
    public Task<CurrentUserOrApplication> GetCurrentUserOrApplicationAsync(CancellationToken cancellationToken)
    {
        return userService.GetCurrentUserOrApplicationAsync(cancellationToken);
    }

    public Task<bool> IsAuthenticated(CancellationToken cancellationToken)
    {
        return userService.UserOrApplicationAsync(
            user => Task.FromResult(user is not null),
            application => Task.FromResult(application is not null),
            cancellationToken
        );
    }

    public Task<bool> IsDatabaseOperator(CancellationToken cancellationToken)
    {
        return userService.UserOrApplicationAsync(
            user => Task.FromResult(
                user is not null
                && user.IsAtLeastAssistantManagerOfDatabaseOperator()
            ),
            application => Task.FromResult(
                application is not null
                && application.IsOwnedByDatabaseOperator()
            ),
            cancellationToken
        );
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