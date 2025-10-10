using System;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Database.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using Quartz;

namespace Database.Configuration;

// Inspired by https://github.com/openiddict/openiddict-samples/blob/dev/samples/Velusia/Velusia.Client/Startup.cs
// https://github.com/openiddict/openiddict-samples/blob/855c31f91d6bf5cde735ef3f96fcc3c015b51d79/samples/Velusia/Velusia.Client/Startup.cs
public abstract class AuthConfiguration
{
    public const string ReadApiScope = "api:read";
    public const string WriteApiScope = "api:write";

    public static void ConfigureServices(
        IServiceCollection services,
        IWebHostEnvironment environment,
        AppSettings appSettings
    )
    {
        var encryptionCertificate = LoadCertificate("jwt-encryption-certificate.pfx",
            appSettings.JsonWebToken.EncryptionCertificatePassword);
        var signingCertificate = LoadCertificate("jwt-signing-certificate.pfx",
            appSettings.JsonWebToken.SigningCertificatePassword);
        ConfigureAuthenticationAndAuthorizationServices(services);
        ConfigureTaskScheduling(services, environment);
        ConfigureOpenIddictServices(services, environment, appSettings, encryptionCertificate, signingCertificate);
    }

    private static X509Certificate2 LoadCertificate(
        string fileName,
        string password
    )
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException($"Empty password for certificate {fileName}.");
        }

        var stream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream($"Database.{fileName}")
            ?? throw new ArgumentException($"Missing certificate {fileName}.");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return X509CertificateLoader.LoadPkcs12(
            buffer.ToArray(),
            password,
            X509KeyStorageFlags.EphemeralKeySet
        );
    }

    private static void ConfigureAuthenticationAndAuthorizationServices(
        IServiceCollection services
    )
    {
        // Dot not use the single authentication scheme as the default scheme
        // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-7.0#defaultscheme
        AppContext.SetSwitch("Microsoft.AspNetCore.Authentication.SuppressAutoDefaultScheme", true);
        // Inspired by https://github.com/openiddict/openiddict-samples/blob/01cb2ce4600cab15867e34826b0287622e6dd71b/samples/Velusia/Velusia.Client/Startup.cs
        services
            .AddAuthentication(_ =>
            {
                // To make the various authentication control flows obvious, do
                // not use default schemes for anything and always be explicit
                // instead.
                _.DefaultAuthenticateScheme = null;
                _.DefaultChallengeScheme = null;
                _.DefaultForbidScheme = null;
                _.DefaultScheme = null;
                _.DefaultSignInScheme = null;
                _.DefaultSignOutScheme = null;
            })
            .AddCookie(_ =>
            {
                _.AccessDeniedPath = "/unauthorized";
                _.LoginPath = "/connect/login";
                _.LogoutPath = "/connect/logout";
                _.ReturnUrlParameter = "returnTo";
                _.ExpireTimeSpan = TimeSpan.FromMinutes(50);
                _.SlidingExpiration = false;
            });
    }

    private static void ConfigureTaskScheduling(
        IServiceCollection services,
        IWebHostEnvironment environment
    )
    {
        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
        // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
        // For configuring Quartz see
        // https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/hosted-services-integration.html
        services.AddQuartz(_ =>
        {
            _.SchedulerId = "database";
            _.SchedulerName = "Database";
            _.UseSimpleTypeLoader();
            _.UseInMemoryStore();
            _.UseDefaultThreadPool(_ =>
                _.MaxConcurrency = 10
            );
            if (environment.IsEnvironment(Program.TestEnvironment))
            {
                // See https://gitter.im/MassTransit/MassTransit?at=5db2d058f6db7f4f856fb404
                _.SchedulerName = Guid.NewGuid().ToString();
            }
        });
        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(_ =>
            _.WaitForJobsToComplete = true
        );
    }

    private static void ConfigureOpenIddictServices(
        IServiceCollection services,
        IWebHostEnvironment environment,
        AppSettings appSettings,
        X509Certificate2 encryptionCertificate,
        X509Certificate2 signingCertificate
    )
    {
        services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(_ =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                _.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
                // Enable Quartz.NET integration.
                _.UseQuartz();
            })
            .AddValidation(_ =>
            {
                // The validation handler uses OpenID Connect discovery to
                // retrieve the issuer signing keys used to validate tokens.
                _.SetIssuer(new Uri(appSettings.MetabaseHost, UriKind.Absolute));
                // Configure the audience accepted by this resource server.
                _.AddAudiences(appSettings.OpenIdConnectClient.Id);
                // Configure the validation handler to use introspection and
                // register the client credentials used when communicating with
                // the remote introspection endpoint.
                // https://www.oauth.com/oauth2-servers/token-introspection-endpoint/
                _.UseIntrospection()
                    .SetClientId(appSettings.OpenIdConnectClient.Id)
                    .SetClientSecret(appSettings.OpenIdConnectClient.Secret);
                // Register the ASP.NET Core host.
                _.UseAspNetCore();
                // Register the System.Net.Http integration.
                _.UseSystemNetHttp()
                    .ConfigureHttpClientHandler(handler => {
                        if (environment.IsDevelopment()) {
                            // https://documentation.openiddict.com/integrations/system-net-http#register-a-custom-httpclienthandler-configuration-delegate
                            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        }
                    });
            })
            .AddClient(_ =>
            {
                _.AllowAuthorizationCodeFlow();

                // Register the signing and encryption credentials.
                // See https://stackoverflow.com/questions/50862755/signing-keys-certificates-and-client-secrets-confusion/50932120#50932120
                _.AddEncryptionCertificate(encryptionCertificate)
                    .AddSigningCertificate(signingCertificate);

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                _.UseAspNetCore()
                    .EnableStatusCodePagesIntegration() // https://documentation.openiddict.com/integrations/aspnet-core#status-code-pages-middleware-integration
                    .EnableRedirectionEndpointPassthrough() // https://documentation.openiddict.com/integrations/aspnet-core#pass-through-mode
                    .EnablePostLogoutRedirectionEndpointPassthrough();
                // .DisableTransportSecurityRequirement(); // https://documentation.openiddict.com/integrations/aspnet-core#transport-security-requirement

                // Register the System.Net.Http integration and use the identity of the current
                // assembly as a more specific user agent, which can be useful when dealing with
                // providers that use the user agent as a way to throttle requests (e.g Reddit).
                _.UseSystemNetHttp()
                    .SetProductInformation(typeof(Startup).Assembly)
                    .ConfigureHttpClientHandler(handler => {
                        if (environment.IsDevelopment()) {
                            // https://documentation.openiddict.com/integrations/system-net-http#register-a-custom-httpclienthandler-configuration-delegate
                            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        }
                    });

                // Add a client registration matching the client application definition in the server project.
                _.AddRegistration(
                    new OpenIddictClientRegistration
                    {
                        Issuer = new Uri(appSettings.MetabaseHost, UriKind.Absolute),

                        // Note: these settings must match the application details
                        // inserted in the database at the server level.
                        ClientId = appSettings.OpenIdConnectClient.Id,
                        ClientSecret = appSettings.OpenIdConnectClient.Secret,

                        // https://auth0.com/docs/get-started/apis/scopes/openid-connect-scopes#standard-claims
                        Scopes =
                        {
                            OpenIddictConstants.Scopes.Address,
                            OpenIddictConstants.Scopes.Email,
                            OpenIddictConstants.Scopes.Phone,
                            OpenIddictConstants.Scopes.Profile,
                            OpenIddictConstants.Scopes.Roles,
                            ReadApiScope,
                            WriteApiScope
                        },

                        // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
                        // URI per provider, unless all the registered providers support returning a special "iss"
                        // parameter containing their URL as part of authorization responses. For more information,
                        // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                        RedirectUri = new Uri("connect/callback/login/metabase", UriKind.Relative),
                        PostLogoutRedirectUri = new Uri("connect/callback/logout/metabase", UriKind.Relative)
                    }
                );
            });
    }
}