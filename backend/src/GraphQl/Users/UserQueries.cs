using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Services;
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
        if (!claimsPrincipal.HasClaim(ClaimTypes.NameIdentifier))
        {
            return null;
        }
        return
            await context.Users.AsNoTracking()
                .SingleOrDefaultAsync(
                    u => claimsPrincipal.GetClaims(ClaimTypes.NameIdentifier).Contains(u.Subject),
                    cancellationToken
                );
    }

    public Task<UserInfo> GetCurrentUserInfoAsync(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GraphQlRequestHelper.TransformExceptionsAsync(
            () => GetUserInfo.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            ),
            resolverContext,
            GetUserInfo.GetEndpoint(appSettings)
        );
    }
}