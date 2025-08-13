using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OpenIddict.Validation.AspNetCore;

namespace Database.Configuration;

public static class HttpContextAuthentication
{
    public static async Task<AuthenticateResult> Authenticate(
        HttpContext httpContext
    )
    {
        // For the Next.js Web frontend, the database acts as OpenId Connect
        // Client and uses the cookie scheme for authentication. See
        // `AuthConfiguration#ConfigureAuthenticationAndAuthorizationServices`
        // for the configuration of the "cookie scheme" cookie. This cookie
        // is set by methods in `AuthenticationController` and is related to
        // `OpenIddictBuilder#AddClient` in
        // `AuthConfiguration#ConfigureOpenIddictServices`.
        var cookieAuthenticateResult = await httpContext
            .AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (cookieAuthenticateResult.Succeeded && cookieAuthenticateResult.Principal is not null)
        {
            httpContext.User = cookieAuthenticateResult.Principal;
            return cookieAuthenticateResult;
        }

        // For third-party frontends, the database acts as resource server
        // and uses authorization-header bearer tokens for authentication,
        // that is JavaScript Web Tokens (JWT), aka, Access Tokens, provided
        // as `Authorization` HTTP header with the prefix `Bearer` as issued
        // by OpenIddict. This Access Token includes Scopes and Claims. The
        // scheme is configured in
        // `AuthConfiguration#ConfigureOpenIddictServices` by
        // `OpenIddictBuilder#AddValidation`.
        var jwtAuthenticateResult = await httpContext
            .AuthenticateAsync(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        if (jwtAuthenticateResult.Succeeded && jwtAuthenticateResult.Principal is not null)
        {
            httpContext.User = jwtAuthenticateResult.Principal;
            return jwtAuthenticateResult;
        }

        return AuthenticateResult.Fail("All available authentication schemes failed or yielded no claims principal.");
    }
}