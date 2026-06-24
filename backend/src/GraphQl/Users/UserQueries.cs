using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using static Database.ApiRequests.GetUserInfo;

namespace Database.GraphQl.Users;

[ExtendObjectType(nameof(Query))]
public sealed class UserQueries
{
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public Task<QueryCurrentUserOrApplication.CurrentUser?> GetCurrentUserAsync(
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return authorization.SwitchUserOrApplicationAsync(
            user => Task.FromResult(user),
            application => Task.FromResult<QueryCurrentUserOrApplication.CurrentUser?>(null),
            cancellationToken
        );
    }

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public Task<UserInfo> GetCurrentUserInfoAsync(
        GetUserInfo getUserInfo,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GraphQlRequestHelper.TransformExceptionsAsync(
            () => getUserInfo.Do(
                cancellationToken
            ),
            resolverContext,
            getUserInfo.GetEndpoint
        );
    }
}