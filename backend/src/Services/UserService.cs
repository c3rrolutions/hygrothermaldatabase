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
    public string? GetOpenIdConnectApplicationClientId()
    {
        // `client_id` (Standard OAuth2 Claim) is the client ID of the application that requested the access token
        // `azp` (Authorized Party - Standard OpenID Connect Claim) is the the client ID of the application to which the ID token or access token was issued
        // `oi_prst` (OpenIddict Presenter - Private Internal Claim) is OpenIddict's proprietary, internal equivalent to `client_id` or `azp`
        return httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.ClientId)
            ?? httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.AuthorizedParty)
            ?? httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.Private.Presenter);
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