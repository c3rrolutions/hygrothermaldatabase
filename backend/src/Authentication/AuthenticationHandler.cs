using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;

namespace Database.Authentication;

public sealed class AuthenticationHandler(
    OpenIddictClientService openIddictClientService,
    UserService userService
)
{
    private static async Task UpdateTokenValuesAsync(
          HttpContext httpContext,
          ClaimsPrincipal claimsPrincipal,
          AuthenticationProperties authenticationProperties,
          AuthenticationTokens freshTokens
      )
    {
        // Override the tokens using the values returned in the token response.
        var properties = authenticationProperties.Clone();
        properties.UpdateTokenValue(
            AuthenticationTokens.AccessTokenName,
            freshTokens.AccessToken
        );
        properties.UpdateTokenValue(
            AuthenticationTokens.AccessTokenExpirationDateName,
            freshTokens.AccessTokenExpirationDateAsString ?? string.Empty
        );
        // Note: if no refresh token was returned, preserve the refresh token initially returned.
        if (!string.IsNullOrEmpty(freshTokens.RefreshToken))
        {
            properties.UpdateTokenValue(
                AuthenticationTokens.RefreshTokenName,
                freshTokens.RefreshToken
            );
        }
        // Remove the redirect URI from the authentication properties
        // to prevent the cookies handler from genering a 302 response.
        properties.RedirectUri = null;
        // Set the creation and expiration dates of the ticket to null to decorrelate the lifetime
        // of the resulting authentication cookie from the lifetime of the identity token returned by
        // the authorization server (if applicable). In this case, the expiration date time will be
        // automatically computed by the cookie handler using the lifetime configured in the options.
        //
        // Applications that prefer binding the lifetime of the ticket stored in the authentication cookie
        // to the identity token returned by the identity provider can remove or comment these two lines:
        properties.IssuedUtc = null;
        properties.ExpiresUtc = null;
        // Note: this flag controls whether the authentication cookie that will be returned to the
        // browser will be treated as a session cookie (i.e destroyed when the browser is closed)
        // or as a persistent cookie. In both cases, the lifetime of the authentication ticket is
        // always stored as protected data, preventing malicious users from trying to use an
        // authentication cookie beyond the lifetime of the authentication ticket itself.
        properties.IsPersistent = false;
        // If multiple HTTP responses for the same user are returned in parallel, the browser will
        // always store the latest cookie received and the refresh tokens stored in the other cookies
        // will be discarded.
        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            properties
        );
    }

    private async Task<string?> FetchAndRefreshAccessTokenFromCookieAuthenticationAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken
    )
    {
        var cookieAuthenticationResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (cookieAuthenticationResult is not { Succeeded: true, Principal.Identity.IsAuthenticated: true })
        {
            return null;
        }
        var accessToken = AuthenticationTokens.ExtractAccessToken(cookieAuthenticationResult);
        var expirationDate = AuthenticationTokens.ExtractAccessTokenExpirationDate(cookieAuthenticationResult);
        if (accessToken is not null
            && expirationDate is not null
            && TimeProvider.System.GetUtcNow() <= expirationDate?.Subtract(OpenIdConnectConstants.AccessTokenLifetime.Divide(3))
        )
        {
            return accessToken;
        }
        var refreshToken = AuthenticationTokens.ExtractRefreshToken(cookieAuthenticationResult);
        if (refreshToken is not null)
        {
            var refreshTokenAuthenticationResult = await openIddictClientService.AuthenticateWithRefreshTokenAsync(
                new OpenIddictClientModels.RefreshTokenAuthenticationRequest
                {
                    DisableUserInfo = true,
                    RefreshToken = refreshToken,
                    CancellationToken = cancellationToken,
                }
            );
            await UpdateTokenValuesAsync(
                httpContext,
                cookieAuthenticationResult.Principal,
                cookieAuthenticationResult.Properties,
                new AuthenticationTokens(
                    AccessToken: refreshTokenAuthenticationResult.AccessToken,
                    AccessTokenExpirationDate: refreshTokenAuthenticationResult.AccessTokenExpirationDate,
                    RefreshToken: refreshTokenAuthenticationResult.RefreshToken
                )
            );
            return refreshTokenAuthenticationResult.AccessToken;
        }
        return null;
    }

    public async Task<AuthenticateResult> AuthenticateAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken
    )
    {
        if (AuthenticationHelpers.IsSameOriginOrReferer(httpContext.Request))
        {
            // For the Next.js Web frontend, the database acts as OpenId Connect
            // Client and uses the cookie scheme for authentication. See
            // `AuthConfiguration#ConfigureAuthenticationAndAuthorizationServices`
            // for the configuration of the "cookie scheme" cookie. This cookie
            // is set by methods in `AuthenticationController` and is related to
            // `OpenIddictBuilder#AddClient` in
            // `AuthConfiguration#ConfigureOpenIddictServices`.
            var accessToken = await FetchAndRefreshAccessTokenFromCookieAuthenticationAsync(
                httpContext,
                cancellationToken
            );
            if (accessToken is not null)
            {
                httpContext.SetBearerToken(accessToken);
            }
        }
        // For third-party frontends, the database acts as resource server
        // and uses authorization-header bearer tokens for authentication,
        // that is JavaScript Web Tokens (JWT), aka, Access Tokens, provided
        // as `Authorization` HTTP header with the prefix `Bearer` as issued
        // by OpenIddict. This Access Token includes Scopes and Claims. The
        // scheme is configured in
        // `AuthConfiguration#ConfigureOpenIddictServices` by
        // `OpenIddictBuilder#AddValidation`.
        //
        // First, try to authenticate the possibly-existing bearer token
        // through the registered OpenID Connect Client (see `AddClient` in
        // `AuthConfiguration`). This only succeeds if the bearer token was
        // issued for the registered client (namely
        // `appSettings.OpenIdConnectClient.Id`). In particular, this is the
        // case if the bearer token comes from the cookie scheme above.
        var clientAuthenticateResult = await httpContext.AuthenticateAsync(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        if (clientAuthenticateResult is { Succeeded: true, Principal.Identity.IsAuthenticated: true })
        {
            return clientAuthenticateResult;
        }
        // Secondly, try to authenticate ignoring which client the token was
        // issued to. This is necessary when another application was issued a
        // bearer token from the metabase for its own OpenID Connect Client
        // (another one than `appSettings.OpenIdConnectClient.Id`) and uses
        // this token to authenticate (and authorize) access to this database.
        var metabaseAuthenticateResult = await userService.SwitchUserOrInstitutionAsync(
            user => Task.FromResult(user is null ? null : CreateSuccessfulAuthenticateResult($"user:{user.Uuid.ToString("D")}")),
            institution => Task.FromResult<AuthenticateResult?>(CreateSuccessfulAuthenticateResult($"institution:{institution.Uuid.ToString("D")}")),
            cancellationToken
        );
        if (metabaseAuthenticateResult is { Succeeded: true, Principal.Identity.IsAuthenticated: true })
        {
            return metabaseAuthenticateResult;
        }
        return AuthenticateResult.Fail("Neither authenticated through cookie nor bearer token.");
    }

    private static AuthenticateResult CreateSuccessfulAuthenticateResult(string nameIdentifier)
    {
        return AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        claims: [new Claim(ClaimTypes.NameIdentifier, nameIdentifier)],
                        authenticationType: "Metabase",
                        nameType: ClaimTypes.Name,
                        roleType: ClaimTypes.Role
                    )
                ),
                AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme
            )
        );
    }
}