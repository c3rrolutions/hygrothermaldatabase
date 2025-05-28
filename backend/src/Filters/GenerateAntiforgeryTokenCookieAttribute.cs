using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Filters;

public sealed class GenerateAntiforgeryTokenCookieAttribute : ResultFilterAttribute
{
    public const string RequestVerificationTokenName = "RequestVerificationToken";

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var antiforgery = context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
        // Send the request token as a JavaScript-readable cookie
        var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
        context.HttpContext.Response.Cookies.Append(
            RequestVerificationTokenName,
            tokens.RequestToken ?? throw new ArgumentException("There is no request token."),
            new CookieOptions { HttpOnly = false }
        );
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
    }
}