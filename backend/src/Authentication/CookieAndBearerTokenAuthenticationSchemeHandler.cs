using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Database.Authentication;

public sealed class CookieAndBearerTokenAuthenticationSchemeHandler(
    IOptionsMonitor<CookieAndBearerTokenAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AuthenticationHandler authenticationHandler
)
: AuthenticationHandler<CookieAndBearerTokenAuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return authenticationHandler.AuthenticateAsync(
            Context,
            Context.RequestAborted
        );
    }
}