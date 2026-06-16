using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Database.Authentication;
using Database.Authorization;
using Database.Data;
using Database.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using Quartz;
using Quartz.AspNetCore;

namespace Database.Configuration;

public static class AuthConfiguration
{
    private static readonly TimeSpan s_cookieExpirationTimeSpan = TimeSpan.FromDays(1);

    private static readonly Dictionary<string, string> s_policyNameToOpenIdConnectScope = new()
    {
        { AuthorizationPolicies.ReadScopePolicy, OpenIdConnectScope.ReadApiScope },
        { AuthorizationPolicies.WriteScopePolicy, OpenIdConnectScope.WriteApiScope },
    };

    private static void BootstrapCertificates(IClock clock)
    {
        using var store = new X509Store(OpenIdConnectConstants.CertificateStoreName, OpenIdConnectConstants.CertificateStoreLocation);
        try
        {
            store.Open(OpenFlags.ReadWrite);
            {
                var distinguishedName = OpenIdConnectConstants.SigningSubjectDistinguishedName;
                var certificates = store.Certificates.Find(
                    X509FindType.FindBySubjectDistinguishedName,
                    distinguishedName,
                    validOnly: true
                );
                if (certificates.Count is 0)
                {
                    store.Add(
                        JwtSigningAndEncryptionCertificateRotationJob.CreateSigningCertificate(
                            distinguishedName,
                            clock
                        )
                    );
                }
            }
            {
                var distinguishedName = OpenIdConnectConstants.EncryptionSubjectDistinguishedName;
                var certificates = store.Certificates.Find(
                    X509FindType.FindBySubjectDistinguishedName,
                    distinguishedName,
                    validOnly: true
                );
                if (certificates.Count is 0)
                {
                    store.Add(
                        JwtSigningAndEncryptionCertificateRotationJob.CreateEncryptionCertificate(
                            distinguishedName,
                            clock
                        )
                    );
                }
            }
        }
        finally
        {
            store.Close();
        }
    }

    private static IEnumerable<X509Certificate2> FindCertificates(string distinguishedName)
    {
        using var store = new X509Store(OpenIdConnectConstants.CertificateStoreName, OpenIdConnectConstants.CertificateStoreLocation);
        try
        {
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(
                X509FindType.FindBySubjectDistinguishedName,
                distinguishedName,
                // OpenIddict automatically prioritizes the cert with the latest expiration
                // Expired keys are needed for tokens signed and encrypted before it expired
                validOnly: false
            );
            foreach (var certificate in certificates)
            {
                yield return certificate;
            }
        }
        finally
        {
            store.Close();
        }
    }

    public static void ConfigureServices(
        IServiceCollection services,
        IWebHostEnvironment environment,
        AppSettings appSettings,
        IClock clock
    )
    {
        BootstrapCertificates(clock);
        services.AddScoped<AuthenticationHandler>();
        services.AddScoped<GraphQlAuthenticationAndAntiforgeryHandler>();
        ConfigureAuthenticationAndAuthorizationServices(services);
        ConfigureTaskScheduling(services, environment);
        ConfigureOpenIddictServices(services, environment, appSettings);
        AddAuthorizationServices(services);
    }

    private static void AddAuthorizationServices(
        IServiceCollection services
    )
    {
        services.AddScoped<CommonAuthorization>();
    }


    private static void ConfigureAuthenticationAndAuthorizationServices(
        IServiceCollection services
    )
    {
        // Dot not use the single authentication scheme as the default scheme
        // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-7.0#defaultscheme
        AppContext.SetSwitch("Microsoft.AspNetCore.Authentication.SuppressAutoDefaultScheme", true);
        // Inspired by https://github.com/openiddict/openiddict-samples/blob/01cb2ce4600cab15867e34826b0287622e6dd71b/samples/Velusia/Velusia.Client/Startup.cs
        services.AddAuthentication(_ =>
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
            // The cookie is used by the database acting as client application through the
            // authentication scheme `CookieAuthenticationDefaults.AuthenticationScheme`, that is, "Cookies".
            .AddCookie(_ =>
            {
                _.AccessDeniedPath = "/unauthorized";
                _.LoginPath = "/connect/login";
                _.LogoutPath = "/connect/logout";
                _.ReturnUrlParameter = "returnTo";
                _.ExpireTimeSpan = s_cookieExpirationTimeSpan;
                _.SlidingExpiration = true;
            })
            .AddScheme<
                CookieAndBearerTokenAuthenticationSchemeOptions,
                CookieAndBearerTokenAuthenticationSchemeHandler
            >(
                AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme,
                _ => { }
            );
        services.AddAuthorization(_ =>
            {
                _.AddPolicy(AuthorizationPolicies.AuthenticatedPolicy, policy =>
                    {
                        policy.AuthenticationSchemes =
                        [
                            AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme
                        ];
                        policy.RequireAuthenticatedUser();
                    }
                );
                foreach (var (policyName, scope) in s_policyNameToOpenIdConnectScope)
                {
                    _.AddPolicy(policyName, policy =>
                        {
                            policy.AuthenticationSchemes =
                            [
                                AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme
                            ];
                            policy.RequireAuthenticatedUser();
                            policy.RequireAssertion(context =>
                                {
                                    return context.User.HasScope(scope);
                                }
                            );
                        }
                    );
                }
            }
        );
    }

    private static void ConfigureTaskScheduling(
        IServiceCollection services,
        IWebHostEnvironment environment
    )
    {
        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks (like
        // pruning orphaned authorizations/tokens from the database) at regular intervals. For
        // configuring Quartz see https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/hosted-services-integration.html
        services.AddQuartz(_ =>
        {
            _.SchedulerId = OpenIdConnectConstants.DatabaseQuartzSchedulerId;
            _.SchedulerName = "Database";
            if (environment.IsEnvironment(Program.TestEnvironment))
            {
                var probablyUniqueId = Guid.NewGuid().ToString();
                _.SchedulerId = $"{OpenIdConnectConstants.DatabaseQuartzSchedulerId}-{probablyUniqueId}";
                _.SchedulerName = $"Metabase-{probablyUniqueId}";
            }
            _.UseSimpleTypeLoader();
            _.UseInMemoryStore();
            _.UseDefaultThreadPool(_ =>
                _.MaxConcurrency = 10
            );
            var jwtSigningAndEncryptionKeyRotationJobKey = new JobKey(JwtSigningAndEncryptionCertificateRotationJob.KeyName);
            _.AddJob<JwtSigningAndEncryptionCertificateRotationJob>(_ => _.WithIdentity(jwtSigningAndEncryptionKeyRotationJobKey));
            _.AddTrigger(_ => _
                .ForJob(jwtSigningAndEncryptionKeyRotationJobKey)
                .WithIdentity(JwtSigningAndEncryptionCertificateRotationJob.TriggerIdentityName)
                .StartNow()
                .WithSimpleSchedule(_ => _
                    .WithIntervalInHours(24)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionFireNow()
                )
            );
        });
        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzServer(_ =>
            _.WaitForJobsToComplete = true
        );
    }

    private static void ConfigureOpenIddictServices(
        IServiceCollection services,
        IWebHostEnvironment environment,
        AppSettings appSettings
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
                _.SetIssuer(appSettings.MetabaseHostUri);
                // Configure the audience accepted by this resource server.
                _.AddAudiences(OpenIdConnectConstants.MetabaseClientId);
                // Configure the validation handler to use introspection and
                // register the client credentials used when communicating with
                // the remote introspection endpoint.
                // https://www.oauth.com/oauth2-servers/token-introspection-endpoint/
                _.UseIntrospection()
                    .SetClientId(appSettings.OpenIdConnectClient.Id)
                    .SetClientSecret(appSettings.OpenIdConnectClient.Secret);
                // Register the ASP.NET Core host.
                _.UseAspNetCore();
                // Enable token entry validation: https://documentation.openiddict.com/configuration/token-storage.html#enabling-token-entry-validation-at-the-api-level
                // _.EnableTokenEntryValidation(); // Token entry validation cannot be enabled when using introspection.
                // Enable authorization entry validation: https://documentation.openiddict.com/configuration/authorization-storage.html#enabling-authorization-entry-validation-at-the-api-level
                // _.EnableAuthorizationEntryValidation(); // Authorization entry validation cannot be enabled when using introspection.
                // Register the System.Net.Http integration.
                _.UseSystemNetHttp()
                    .ConfigureHttpClientHandler(handler =>
                    {
                        if (environment.IsDevelopment())
                        {
                            // https://documentation.openiddict.com/integrations/system-net-http#register-a-custom-httpclienthandler-configuration-delegate
                            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        }
                    });
            })
            .AddClient(_ =>
            {
                _.AllowAuthorizationCodeFlow()
                 .AllowRefreshTokenFlow();

                // Register the signing and encryption credentials. See https://stackoverflow.com/questions/50862755/signing-keys-certificates-and-client-secrets-confusion/50932120#50932120
                foreach (var encryptionCertificate in FindCertificates(OpenIdConnectConstants.EncryptionSubjectDistinguishedName))
                {
                    _.AddEncryptionCertificate(encryptionCertificate);
                }
                foreach (var signingCertificate in FindCertificates(OpenIdConnectConstants.SigningSubjectDistinguishedName))
                {
                    _.AddSigningCertificate(signingCertificate);
                }

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
                    .ConfigureHttpClientHandler(handler =>
                    {
                        if (environment.IsDevelopment())
                        {
                            // https://documentation.openiddict.com/integrations/system-net-http#register-a-custom-httpclienthandler-configuration-delegate
                            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        }
                    });

                // Add a client registration matching the client application definition in the
                // server project.
                var clientRegistration = new OpenIddictClientRegistration
                {
                    RegistrationId = OpenIdConnectConstants.MetabaseRegistrationId,
                    Issuer = appSettings.MetabaseHostUri,

                    // Note: these settings must match the application details inserted in the
                    // database at the server level.
                    ClientId = appSettings.OpenIdConnectClient.Id,
                    ClientSecret = appSettings.OpenIdConnectClient.Secret,

                    // Note: to mitigate mix-up attacks, it's recommended to use a unique
                    // redirection endpoint URI per provider, unless all the registered
                    // providers support returning a special "iss" parameter containing their
                    // URL as part of authorization responses. For more information, see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                    RedirectUri = new UriBuilder(appSettings.Uri) { Path = $"connect/callback/login/{OpenIdConnectConstants.MetabaseClientId}" }.Uri,
                    PostLogoutRedirectUri = new UriBuilder(appSettings.Uri) { Path = $"connect/callback/logout/{OpenIdConnectConstants.MetabaseClientId}" }.Uri
                };
                clientRegistration.Scopes.UnionWith([
                    OpenIddictConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.OpenId,
                    ..OpenIdConnectScope.Scopes
                ]);
                _.AddRegistration(clientRegistration);
            });
    }
}