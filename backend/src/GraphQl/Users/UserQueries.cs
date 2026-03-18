using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static Database.ApiRequests.GetUserInfo;

namespace Database.GraphQl.Users;

[ExtendObjectType(nameof(Query))]
public sealed class UserQueries
{
    public async Task<User?> GetCurrentUserAsync(
        ClaimsPrincipal claimsPrincipal,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!claimsPrincipal.HasClaim(OpenIddictConstants.Claims.Subject))
        {
            return null;
        }
        return
            await context.Users.AsNoTracking()
                .SingleOrDefaultAsync(
                    u => claimsPrincipal.GetClaims(OpenIddictConstants.Claims.Subject).Contains(u.Subject),
                    cancellationToken
                );
    }

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