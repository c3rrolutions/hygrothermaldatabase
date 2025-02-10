using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OpenIddict.Client.AspNetCore;

namespace Database.Extensions;

public static class HttpContextAccessorExtensions
{
    public static async Task<string?> ExtractBearerToken(this IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null) return null;
        return await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken
        ).ConfigureAwait(false);
    }
}