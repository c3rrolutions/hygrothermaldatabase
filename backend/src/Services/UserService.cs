using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.ApiRequests.Dto;
using Database.Extensions;
using Database.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

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
    IHttpClientFactory httpClientFactory,
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
    /// Get current user from Metabase.
    /// </summary>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> </returns>
    public async Task<CurrentUserDto?> GetCurrentUser(CancellationToken cancellationToken)
    {
        // Extract token
        var token = await httpContextAccessor.ExtractBearerToken();
        logger.ExtractedToken(token);
        if (token is null)
        {
            return await GetCurrentUserFromMetabase(cancellationToken);
        }
        // Check if there is already a user for token
        if (!cacheService.TryGetUser(token, out var cacheUser))
        {
            // Get user from Metabase
            cacheUser = await GetCurrentUserFromMetabase(cancellationToken);
            // Store user in cache
            cacheService.SetUser(token, cacheUser);
        }
        return cacheUser;
    }

    private Task<CurrentUserDto?> GetCurrentUserFromMetabase(CancellationToken cancellationToken)
    {
        return UserApi.RequestCurrentUser(
            appSettings,
            apiRequestService,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }
}