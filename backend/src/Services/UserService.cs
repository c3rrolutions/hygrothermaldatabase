using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Database.ApiRequests;
using Database.Extensions;
using static Database.ApiRequests.QueryCurrentUser;

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
    ILogger<UserService> logger)
{
    /// <summary>
    /// Get application from user calims.
    /// </summary>
    /// <returns> ClientId from claims as applicationId. </returns>
    public string? GetOpenIdConnectClientId()
    {
        return httpContextAccessor.HttpContext?.User.GetClaim(Claims.AuthorizedParty);
    }

    /// <summary>
    /// Get current user from Metabase (or the local cache).
    /// </summary>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> </returns>
    public async Task<CurrentUser?> GetCurrentUser(CancellationToken cancellationToken)
    {
        // Extract token
        var token = await httpContextAccessor.ExtractBearerToken();
        logger.ExtractedToken(token);
        if (token is null)
        {
            return await QueryCurrentUser.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            );
        }
        // Check if there is already a user for token
        if (!cacheService.TryGetUser(token, out var cacheUser))
        {
            // Get user from Metabase
            cacheUser = await QueryCurrentUser.Do(
                appSettings,
                apiRequestService,
                cancellationToken
            );
            // Store user in cache
            cacheService.SetUser(token, cacheUser);
        }
        return cacheUser;
    }
}