using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.ApiRequests.Dto;
using Database.Extensions;
using Database.Logging;
using Database.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Database.Services;

/// <summary>
/// Implementation of <see cref="IUserService"/>
/// </summary>
/// <param name="appSettings">         <see cref="AppSettings"/> </param>
/// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
/// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
/// <param name="cacheService">        <see cref="ICacheService"/> to store already known users. </param>
/// <param name="logger">              Instance of <see cref="ILogger"/> </param>
public class UserService(
    AppSettings appSettings,
    IApiRequestService apiRequestService,
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    ICacheService cacheService,
    ILogger<IUserService> logger) : IUserService
{
    /// <inheritdoc/>
    public string? GetApplicationIdFromUser()
    {
        return httpContextAccessor.HttpContext?.User.GetClaim(Claims.ClientId);
    }

    /// <inheritdoc/>
    public async Task<CurrentUserDto?> GetCurrentUser(CancellationToken cancellationToken)
    {
        // Extract token
        var token = await httpContextAccessor.ExtractBearerToken().ConfigureAwait(false);
        logger.ExtractedToken(token);

        if (token is null)
        {
            return await GetCurrentUserFromMetabase(cancellationToken).ConfigureAwait(false);
        }

        // Check if there is already a user for token
        if (!cacheService.TryGetUser(token, out CurrentUserDto? cacheUser))
        {
            // Get user from Metabase
            cacheUser = await GetCurrentUserFromMetabase(cancellationToken).ConfigureAwait(false);
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
            cancellationToken);
    }
}