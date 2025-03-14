using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest;
using Database.ApiRequest.Dto;
using Database.Extensions;
using Database.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Database.Services;

/// <summary>
/// Implementation of <see cref="IUserService"/>
/// </summary>
/// <param name="appSettings">       <see cref="AppSettings"/> </param>
/// <param name="httpClientFactory"> <see cref="IHttpClientFactory"/> </param>
/// <param name="cache">             <see cref="IMemoryCache"/> to store already known users. </param>
/// <param name="logger">            Instance of <see cref="ILogger"/> </param>
public class UserService(
    AppSettings appSettings,
    IApiRequestService apiRequestService,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    ILogger<IUserService> logger) : IUserService
{
    /// <inheritdoc/>
    public async Task<CurrentUserDto?> GetCurrentUser(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    {
        // Extract token
        var token = await httpContextAccessor.ExtractBearerToken().ConfigureAwait(false);
        logger.ExtractedToken(token);

        if (token == null)
        {
            return await GetCurrentUserFromMetabase(httpContextAccessor, cancellationToken).ConfigureAwait(false);
        }

        // Check if there is already a user for token
        if (!cache.TryGetValue(token, out CurrentUserDto? cacheUser))
        {
            // Get user from Metabase
            cacheUser = await GetCurrentUserFromMetabase(httpContextAccessor, cancellationToken).ConfigureAwait(false);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            // Store user in cache
            cache.Set(token, cacheUser, cacheEntryOptions);
        }

        return cacheUser;
    }

    private Task<CurrentUserDto?> GetCurrentUserFromMetabase(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    {
        return UserApi.RequestCurrentUser(
            appSettings,
            apiRequestService,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken);
    }
}