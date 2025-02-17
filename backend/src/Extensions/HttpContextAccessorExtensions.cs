using System;
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
        var token = await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
        var expirationDate = await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate);

        return !String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(expirationDate) && CheckExpirationDate(expirationDate) ? token : null;
    }

    private static bool CheckExpirationDate(string expirationDate)
    {
        return DateTime.Parse(expirationDate) < DateTime.UtcNow;
    }
}