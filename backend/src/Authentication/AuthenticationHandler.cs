using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Client.OpenIddictClientModels;

namespace Database.Authentication;

public sealed class AuthenticationHandler(
    OpenIddictClientService openIddictClientService
)
{
    private static string? GetBackchannelAccessToken(AuthenticateResult authenticateResult) =>
        authenticateResult.Properties?.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);

    private static DateTimeOffset? GetBackchannelAccessTokenExpirationDate(AuthenticateResult authenticateResult) =>
        DateTimeOffset.TryParse(
            authenticateResult.Properties?.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate),
            CultureInfo.InvariantCulture,
            out var date
        )
        ? date
        : null;

    private static string? GetRefreshToken(AuthenticateResult authenticateResult) =>
        authenticateResult.Properties?.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken);

    private static async Task SetAuthenticationTokensAsync(
          HttpContext httpContext,
          ClaimsPrincipal claimsPrincipal,
          AuthenticationProperties authenticationProperties,
          AuthenticationTokens freshTokens
      )
    {
        // Override the tokens using the values returned in the token response.
        var properties = authenticationProperties.Clone();
        // Keep in sync with `GetTokenValue` in `AddRequestTransform` above
        properties.UpdateTokenValue(
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken,
            freshTokens.AccessToken
        );
        properties.UpdateTokenValue(
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate,
            freshTokens.AccessTokenExpirationDate?.ToString(CultureInfo.InvariantCulture) ?? string.Empty
        );
        properties.UpdateTokenValue(
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken,
            freshTokens.IdentityToken
        );
        // Note: if no refresh token was returned, preserve the refresh token initially returned.
        if (!string.IsNullOrEmpty(freshTokens.RefreshToken))
        {
            properties.UpdateTokenValue(
                OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken,
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

    private static void SetBearerToken(HttpContext httpContext, string accessToken) =>
        httpContext.Request.Headers.Authorization = $"{OpenIddictConstants.Schemes.Bearer} {accessToken}";

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
        var accessToken = GetBackchannelAccessToken(cookieAuthenticationResult);
        var expirationDate = GetBackchannelAccessTokenExpirationDate(cookieAuthenticationResult);
        if (accessToken is not null
            && expirationDate is not null
            && TimeProvider.System.GetUtcNow() <= expirationDate?.Subtract(OpenIdConnectConstants.AccessAndIdentityTokenLifetime.Divide(3))
        )
        {
            return accessToken;
        }
        var refreshToken = GetRefreshToken(cookieAuthenticationResult);
        if (refreshToken is not null)
        {
            var refreshTokenAuthenticationResult = await openIddictClientService.AuthenticateWithRefreshTokenAsync(
                new RefreshTokenAuthenticationRequest
                {
                    DisableUserInfo = true,
                    RefreshToken = refreshToken,
                    CancellationToken = cancellationToken,
                }
            );
            await SetAuthenticationTokensAsync(
                httpContext,
                cookieAuthenticationResult.Principal,
                cookieAuthenticationResult.Properties,
                new AuthenticationTokens(
                    AccessToken: refreshTokenAuthenticationResult.AccessToken,
                    AccessTokenExpirationDate: refreshTokenAuthenticationResult.AccessTokenExpirationDate,
                    IdentityToken: refreshTokenAuthenticationResult.IdentityToken,
                    RefreshToken: refreshTokenAuthenticationResult.RefreshToken
                )
            );
            return refreshTokenAuthenticationResult.AccessToken;
        }
        return null;
    }

    // Inspired by https://github.com/dotnet/aspnetcore/blob/85565dbb30d724c955179e1989c109c677e7b263/src/Http/Http.Extensions/src/RequestHeaders.cs#L338-L342
    public static Uri? GetOrigin(RequestHeaders headers)
    {
        if (Uri.TryCreate(headers.Headers.Origin, UriKind.RelativeOrAbsolute, out var uri))
        {
            return uri;
        }
        return null;
    }

    private static bool IsSameOriginOrReferer(HttpRequest request)
    {
        var headers = request.GetTypedHeaders();
        var originOrReferer = GetOrigin(headers) ?? headers.Referer;
        if (originOrReferer is null)
        {
            return true;
        }
        return request.Host == HostString.FromUriComponent(originOrReferer);
    }

    public async Task<AuthenticateResult> AuthenticateAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken
    )
    {
        if (IsSameOriginOrReferer(httpContext.Request))
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
                SetBearerToken(httpContext, accessToken);
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
        var bearerAuthenticateResult = await httpContext.AuthenticateAsync(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        if (bearerAuthenticateResult is { Succeeded: true, Principal.Identity.IsAuthenticated: true })
        {
            return bearerAuthenticateResult;
        }
        return AuthenticateResult.Fail("All available authentication schemes failed or yielded no claims principal.");
    }
}