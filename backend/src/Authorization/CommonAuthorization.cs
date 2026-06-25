using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using Database.ApiRequests;
using HotChocolate.Resolvers;
using HotChocolate;

namespace Database.Authorization;

public sealed class CommonAuthorization(
    UserService userService
)
{
    // This is the same code that HotChocolate returns when autheorization via the attribute `[Authorize(Policy = ...)]` fails.
    private const string UNAUTHORIZED_CODE = "AUTH_NOT_AUTHENTICATED";

    public Task<T> SwitchUserOrApplicationAsync<T>(
        Func<QueryCurrentUserOrApplication.CurrentUser?, Task<T>> handleUser,
        Func<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication, Task<T>> handleApplication,
        CancellationToken cancellationToken
    )
    {
        return userService.SwitchUserOrApplicationAsync(handleUser, handleApplication, cancellationToken);
    }

    public Task<QueryCurrentUserOrApplication.CurrentUserOrApplication> FetchCurrentUserOrApplicationAsync(CancellationToken cancellationToken)
    {
        return userService.FetchCurrentUserOrApplicationAsync(cancellationToken);
    }

    public Task<bool> IsAuthenticated(CancellationToken cancellationToken)
    {
        return userService.SwitchUserOrApplicationAsync(
            user => Task.FromResult(user is not null),
            application => Task.FromResult(application is not null),
            cancellationToken
        );
    }

    public Task<bool> IsDatabaseOperator(CancellationToken cancellationToken)
    {
        return userService.SwitchUserOrApplicationAsync(
            user => Task.FromResult(
                user is not null
                && user.IsAtLeastAssistantManagerOfDatabaseOperator()
            ),
            application => Task.FromResult(
                application.Owner.IsDatabaseOperator()
            ),
            cancellationToken
        );
    }

    public bool IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(
        QueryCurrentUserOrApplication.CurrentUser currentUser,
        Guid institutionId
    )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t =>
            (
                t.Role is QueryCurrentUserOrApplication.InstitutionRepresentativeRole.ASSISTANT
                || t.Role is QueryCurrentUserOrApplication.InstitutionRepresentativeRole.OWNER
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

    public void ReportUnauthorizedError(
        IResolverContext resolverContext
    )
    {
        resolverContext.ReportError(
            ErrorBuilder.New()
                .SetCode(UNAUTHORIZED_CODE)
                .SetPath(resolverContext.Path)
                .SetMessage($"The current user is not authorized to access this resource.")
                .Build()
        );
    }
}