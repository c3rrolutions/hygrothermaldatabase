using System;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Extensions;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Database.Services;

/// <summary>
/// Service to fetch current user or institution from the metabase or the local cache
/// </summary>
public sealed class UserService(
    QueryCurrentUserOrInstitution queryCurrentUserOrInstitution,
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

    public async Task<T> SwitchUserOrInstitutionAsync<T>(
        Func<QueryCurrentUserOrInstitution.CurrentUser?, Task<T>> handleUser,
        Func<QueryCurrentUserOrInstitution.CurrentInstitution, Task<T>> handleInstitution,
        CancellationToken cancellationToken
    )
    {
        var userOrInstitution = await FetchCurrentUserOrInstitutionAsync(cancellationToken);
        if (userOrInstitution.CurrentInstitution is not null)
        {
            return await handleInstitution(userOrInstitution.CurrentInstitution);
        }
        else
        {
            return await handleUser(userOrInstitution.CurrentUser);
        }
    }

    public async Task<QueryCurrentUserOrInstitution.CurrentUserOrInstitution> FetchCurrentUserOrInstitutionAsync(
        CancellationToken cancellationToken
    )
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return QueryCurrentUserOrInstitution.Empty;
        }
        var token = httpContext.ExtractBearerToken();
        if (token is null)
        {
            return QueryCurrentUserOrInstitution.Empty;
        }
        // If there is no authenticated user, then the bearer token may be
        // invalid or expired, so we cannot trust a possibly-cached result.
        if (httpContext.User is null)
        {
            return await queryCurrentUserOrInstitution.Do(
                cancellationToken
            );
        }
        // If there is an authenticated user, then the bearer token is valid and
        // we try to get the cached user or application.
        if (!cacheService.TryGetCurrentUserOrInstitution(token, out var cachedUserOrInstitution))
        {
            // If it is not cached, fetch it ...
            cachedUserOrInstitution = await queryCurrentUserOrInstitution.Do(
                cancellationToken
            );
            // ... and store it in the cache.
            cacheService.SetCurrentUserOrInstitution(token, cachedUserOrInstitution);
        }
        return cachedUserOrInstitution;
    }
}