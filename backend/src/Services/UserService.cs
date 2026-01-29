using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace Database.Services;

/// <summary>
/// Service to get current user from Metabase
/// </summary>
/// <param name="appSettings">         <see cref="AppSettings"/> </param>
/// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
/// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
/// <param name="cacheService">        <see cref="CacheService"/> to store already known users. </param>
/// <param name="logger">              Instance of <see cref="ILogger"/> </param>
public sealed class UserService(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    IHttpContextAccessor httpContextAccessor,
    CacheService cacheService
)
{
    /// <summary>
    /// Get client ID from user claims.
    /// </summary>
    public string? GetOpenIdConnectClientId()
    {
        return httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.AuthorizedParty);
    }

    public async Task<T> UserOrApplicationAsync<T>(
        Func<QueryCurrentUserOrApplication.CurrentUser?, Task<T>> handleUser,
        Func<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication, Task<T>> handleApplication,
        CancellationToken cancellationToken
    )
    {
        var userOrApplication = await GetCurrentUserOrApplicationAsync(cancellationToken);
        if (userOrApplication.CurrentApplication is not null)
        {
            return await handleApplication(userOrApplication.CurrentApplication);
        }
        else
        {
            return await handleUser(userOrApplication.CurrentUser);
        }
    }

    /// <summary>
    /// Get current user or OpenId Connect application from Metabase (or the local cache).
    /// </summary>
    public async Task<QueryCurrentUserOrApplication.CurrentUserOrApplication> GetCurrentUserOrApplicationAsync(
        CancellationToken cancellationToken
    )
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return QueryCurrentUserOrApplication.Empty;
        }
        var token = httpContext.ExtractBearerToken();
        if (token is null)
        {
            return QueryCurrentUserOrApplication.Empty;
        }
        // If there is no authenticated user, then the bearer token may be
        // invalid or expired, so we cannot trust a possibly-cached result.
        if (httpContext.User is null)
        {
            return await QueryCurrentUserOrApplication.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            );
        }
        // If there is an authenticated user, then the bearer token is valid and
        // we try to get the cached user or application.
        if (!cacheService.TryGetCurrentUserOrApplication(token, out var cachedUserOrApplication))
        {
            // If it is not cached, fetch it ...
            cachedUserOrApplication = await QueryCurrentUserOrApplication.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            );
            // ... and store it in the cache.
            cacheService.SetCurrentUserOrApplication(token, cachedUserOrApplication);
        }
        return cachedUserOrApplication;
    }
}