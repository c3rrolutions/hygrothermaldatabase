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

public static partial class UserServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Extracted Bearer Token: {Token}")]
    public static partial void ExtractedToken(this ILogger<UserService> logger, string? token);
}

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
    CacheService cacheService,
    ILogger<UserService> logger
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
        // Extract token
        var token = await httpContextAccessor.ExtractBearerToken();
        logger.ExtractedToken(token);
        if (token is null)
        {
            return await QueryCurrentUserOrApplication.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            );
        }
        // Check if there is already a user for token
        if (!cacheService.TryGetCurrentUserOrApplication(token, out var cachedUserOrApplication))
        {
            // Get user from Metabase
            cachedUserOrApplication = await QueryCurrentUserOrApplication.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            );
            // Store user in cache
            cacheService.SetCurrentUserOrApplication(token, cachedUserOrApplication);
        }
        return cachedUserOrApplication;
    }
}