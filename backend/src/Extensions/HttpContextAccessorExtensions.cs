using System;
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
        if (httpContextAccessor.HttpContext is null) return null;
        var token = await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
        var expirationDate = await httpContextAccessor.HttpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate);

        return !String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(expirationDate) && CheckExpirationDate(expirationDate) ? token : null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:IFormatProvider angeben", Justification = "No need at this point.")]
    private static bool CheckExpirationDate(string expirationDate)
    {
        return DateTime.UnixEpoch.AddSeconds(int.Parse(expirationDate)) < DateTime.UtcNow;
    }
}