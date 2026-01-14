using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Database.ApiRequests;
using Database.Extensions;
using static Database.ApiRequests.QueryCurrentUserOrApplication;
using System;

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
        return httpContextAccessor.HttpContext?.User.GetClaim(Claims.AuthorizedParty);
    }

    public async Task<T> UserOrApplicationAsync<T>(
        Func<CurrentUser?, Task<T>> authorizeUser,
        Func<CurrentOpenIdConnectApplication?, Task<T>> authorizeApplication,
        CancellationToken cancellationToken
    )
    {
        var userOrApplication = await GetCurrentUserOrApplicationAsync(cancellationToken);
        if (userOrApplication.CurrentApplication is not null)
        {
            return await authorizeApplication(userOrApplication.CurrentApplication);
        }
        else
        {
            return await authorizeUser(userOrApplication.CurrentUser);
        }
    }

    /// <summary>
    /// Get current user or OpenId Connect application from Metabase (or the local cache).
    /// </summary>
    public async Task<CurrentUserOrApplication> GetCurrentUserOrApplicationAsync(
        CancellationToken cancellationToken
    )
    {
        var httpContext = httpContextAccessor.HttpContext;
        // If there is no HTTP context or there is no authenticated user, return early.
        if (httpContext is null || httpContext.User is null)
        {
            return new CurrentUserOrApplication(null, null);
        }
        // If there is an authenticated user, then the bearer token exists and is valid.
        var token = httpContextAccessor.HttpContext?.ExtractBearerToken();
        if (token is null)
        {
            return new CurrentUserOrApplication(null, null);
        }
        // Try to get the cached user or application for the token.
        if (!cacheService.TryGetCurrentUserOrApplication(token, out var cachedUserOrApplication))
        {
            // If it is not cached, fetch it from the metabase.
            cachedUserOrApplication = await QueryCurrentUserOrApplication.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            );
            // And store it in the cache.
            cacheService.SetCurrentUserOrApplication(token, cachedUserOrApplication);
        }
        return cachedUserOrApplication;
    }
}