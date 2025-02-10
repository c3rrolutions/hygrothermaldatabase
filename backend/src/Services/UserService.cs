using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Metabase;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Database.Services;

public class UserService(
    AppSettings appSettings,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache) : IUserService
{
    public async Task<CurrentUser?> GetCurrentUser(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    {
        var token = await httpContextAccessor.ExtractBearerToken().ConfigureAwait(false);

        if (token == null)
        {
            return await GetCurrentUserFromMetabase(httpContextAccessor, cancellationToken).ConfigureAwait(false);
        }

        if (!cache.TryGetValue(token, out CurrentUser? cacheUser))
        {
            cacheUser = await GetCurrentUserFromMetabase(httpContextAccessor, cancellationToken).ConfigureAwait(false);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));

            cache.Set(token, cacheUser, cacheEntryOptions);
        }

        return cacheUser;
    }

    private async Task<CurrentUser?> GetCurrentUserFromMetabase(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    {
        return await QueryingCurrentUser.Query(
            appSettings,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
    }
}