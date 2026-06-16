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

    public Task<T> SwitchUserOrInstitutionAsync<T>(
        Func<QueryCurrentUserOrInstitution.CurrentUser?, Task<T>> handleUser,
        Func<QueryCurrentUserOrInstitution.CurrentInstitution, Task<T>> handleInstitution,
        CancellationToken cancellationToken
    )
    {
        return userService.SwitchUserOrInstitutionAsync(handleUser, handleInstitution, cancellationToken);
    }

    public Task<QueryCurrentUserOrInstitution.CurrentUserOrInstitution> FetchCurrentUserOrInstitutionAsync(CancellationToken cancellationToken)
    {
        return userService.FetchCurrentUserOrInstitutionAsync(cancellationToken);
    }

    public Task<bool> IsAuthenticated(CancellationToken cancellationToken)
    {
        return userService.SwitchUserOrInstitutionAsync(
            user => Task.FromResult(user is not null),
            institution => Task.FromResult(institution is not null),
            cancellationToken
        );
    }

    public Task<bool> IsDatabaseOperator(CancellationToken cancellationToken)
    {
        return userService.SwitchUserOrInstitutionAsync(
            user => Task.FromResult(
                user is not null
                && user.IsAtLeastAssistantManagerOfDatabaseOperator()
            ),
            institution => Task.FromResult(
                institution is not null
                && institution.IsDatabaseOperator()
            ),
            cancellationToken
        );
    }

    public bool IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(
        QueryCurrentUserOrInstitution.CurrentUser currentUser,
        Guid institutionId
    )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t =>
            (
                t.Role is QueryCurrentUserOrInstitution.InstitutionRepresentativeRole.ASSISTANT
                || t.Role is QueryCurrentUserOrInstitution.InstitutionRepresentativeRole.OWNER
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