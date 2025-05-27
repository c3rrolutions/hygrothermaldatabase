using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OpenIddict.Client.AspNetCore;

namespace Database.Extensions;

/// <summary>
/// Extensions of <see cref="HttpContextAccessor"/>
/// </summary>
public static class HttpContextAccessorExtensions
{
    /// <summary>
    /// Extract bearer token from <see cref="HttpContextAccessor"/>. Only returns token if not expired.
    /// </summary>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <returns> Bearer token or null. </returns>
    public static async Task<string?> ExtractBearerToken(this IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return null;
        }
        var token = await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
        var expirationDate = await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate);
        return expirationDate is not null && LiesInTheFuture(expirationDate) ? token : null;
    }

    private static bool LiesInTheFuture(string expirationDate)
    {
        return DateTime.Parse(expirationDate, CultureInfo.InvariantCulture) < DateTime.UtcNow;
    }
}