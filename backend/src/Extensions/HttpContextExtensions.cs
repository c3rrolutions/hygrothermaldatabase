using System;
using System.Linq;
using Database.Authentication;
using Microsoft.AspNetCore.Http;

namespace Database.Extensions;

public static class HttpContextExtensions
{
    public static void SetBearerToken(this HttpContext httpContext, string accessToken) =>
        httpContext.Request.Headers.Authorization = $"{OpenIdConnectConstants.AuthorizationHeaderBearer} {accessToken}";

    public static string? ExtractBearerToken(this HttpContext httpContext)
    {
        var bearerTokenPrefix = $"{OpenIdConnectConstants.AuthorizationHeaderBearer} ";
        return httpContext.Request?.Headers?.Authorization
            .FirstOrDefault(
                _ => _ is not null
                     && _.TrimStart().StartsWith(bearerTokenPrefix, StringComparison.Ordinal)
            )
            ?.TrimStart()
            ?[bearerTokenPrefix.Length..]
            ?.TrimEnd();
    }
}