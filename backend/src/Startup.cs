using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Configuration;
using Database.Data;
using Database.Data.Extensions;
using Database.Enumerations;
using Database.GraphQl;
using Database.Services;
using Laraue.EfCoreTriggers.PostgreSql.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using NodaTime;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;

namespace Database;

public sealed class Startup(
    IWebHostEnvironment environment,
    IConfiguration configuration
)
{
    private readonly AppSettings _appSettings =
        configuration.Get<AppSettings>(_ =>
        {
            _.BindNonPublicProperties = true;
            _.ErrorOnUnknownConfiguration = false;
        })
        ?? throw new InvalidOperationException("Failed to get application settings from configuration.");

    private readonly IClock _clock = SystemClock.Instance;

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureDatabaseServices(services);
        ConfigureRequestResponseServices(services);
        // ConfigureSessionServices(services); // Not used
        ConfigureTelemetryServices(services);
        services.AddAntiforgery(_ =>
        {
            _.HeaderName = AntiforgeryConstants.HeaderName;
        });
        services
            .AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>();
        ConfigureHttpClientServices(services);
        services.AddHttpContextAccessor();
        services
            .AddHealthChecks()
            .AddApplicationLifecycleHealthCheck()
            .AddDbContextCheck<ApplicationDbContext>();
        services.AddSingleton(_appSettings);
        services.AddSingleton(environment);
        services.AddSingleton<IClock>(_clock);
        // services.AddDatabaseDeveloperPageExceptionFilter();
        ConfigureCustomServices(services);
        ConfigureApiRequests(services);
        AuthConfiguration.ConfigureServices(services, environment, _appSettings, _clock);
        GraphQlConfiguration.ConfigureServices(services, environment);
    }

    private static void ConfigureRequestResponseServices(IServiceCollection services)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#forwarded-headers-middleware-order
        services.Configure<ForwardedHeadersOptions>(_ =>
            {
                _.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto |
                    ForwardedHeaders.XForwardedHost;
                // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#forward-the-scheme-for-linux-and-non-iis-reverse-proxies
                _.KnownIPNetworks.Clear();
                _.KnownProxies.Clear();
            }
        );
        services.AddCors(_ =>
            _.AddPolicy(
                GraphQlConstants.CorsPolicy,
                policy =>
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            )
        );
        services.AddControllersWithViews()
        .AddRazorOptions(_ =>
            {
                // The default location formarts are `/Views/{1}/{0}.cshtml` and `/Views/Shared/{0}.cshtml`.
                // Add the format that ignores the directory structure, that is,
                // leave out `/Views` and the controller name `{1}`. For some
                // reason compiled views land in the root directory. The
                // debugging output of `make logs` when navigating to
                // https://local.buildingenvelopedata.org:4041/connect/logout?...
                // is:
                // Initializing Razor view compiler with compiled view: '/Authorize.cshtml'.
                // Initializing Razor view compiler with compiled view: '/Logout.cshtml'.
                // Initializing Razor view compiler with compiled view: '/Verify.cshtml'.
                // Initializing Razor view compiler with compiled view: '/Error.cshtml'.
                // Initializing Razor view compiler with compiled view: '/_Layout.cshtml'.
                // Initializing Razor view compiler with compiled view: '/_ViewImports.cshtml'.
                // Initializing Razor view compiler with compiled view: '/_ViewStart.cshtml'.
                // View lookup cache miss for view 'Logout' in controller 'Authorization'.
                // Could not find a file for view at path '/Views/Authorization/Logout.cshtml'.
                // Could not find a file for view at path '/Views/Shared/Logout.cshtml'.
                // Located compiled view for view at path '/Logout.cshtml'.
                // Located compiled view for view at path '/_ViewStart.cshtml'.
                // Executing ViewResult, running view Logout.
                // The view path '/Logout.cshtml' was found in 5.0269ms.
                _.ViewLocationFormats.Add("/{0}.cshtml");
                // TODO I consider the flattened structure a bug. How can we solve this?
            }
        );
        services.AddOpenApi(OpenApiConstants.DocumentName, _ =>
        {
            _.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
            _.AddScalarTransformers();
        });
    }

    // private static void ConfigureSessionServices(
    //         IServiceCollection services
    //         )
    // {
    //     // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state#session-state
    //     services.AddDistributedMemoryCache();
    //     services.AddSession(options =>
    //     {
    //         // Set a short timeout for easy testing in development and a long
    //         // one otherwise.
    //         options.IdleTimeout =
    //             environment.IsDevelopment()
    //             ? TimeSpan.FromSeconds(10)
    //             : TimeSpan.FromMinutes(30);
    //         options.Cookie.HttpOnly = true;
    //         // Make the session cookie essential
    //         options.Cookie.IsEssential = true;
    //     });
    // }

    private void ConfigureTelemetryServices(
        IServiceCollection services
    )
    {
        services.AddOpenTelemetry()
            // .WithTracing(_ =>
            // {
            //     _.AddAspNetCoreInstrumentation();
            //     _.AddHttpClientInstrumentation();
            //     _.AddHotChocolateInstrumentation();
            //     _.AddOtlpExporter(_ =>
            //     {
            //         _.Endpoint = _appSettings.OpenTelemetry.GrpcUri;
            //         _.Protocol = OtlpExportProtocol.Grpc;
            //     }
            //     );
            //     if (!environment.IsProduction())
            //     {
            //         _.AddConsoleExporter();
            //     }
            // }
            // )
            .WithMetrics(_ =>
            {
                _.AddAspNetCoreInstrumentation(); // inbound requests
                _.AddHttpClientInstrumentation(); // outbound requests
                _.AddOtlpExporter(_ =>
                {
                    _.Endpoint = _appSettings.OpenTelemetry.GrpcUri;
                    _.Protocol = OtlpExportProtocol.Grpc;
                }
                );
                // if (!environment.IsProduction())
                // {
                //     _.AddConsoleExporter();
                // }
            }
            );
    }

    private void ConfigureDatabaseContext(
        DbContextOptionsBuilder options
        )
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
        connectionStringBuilder.Host = _appSettings.Database.Host;
        connectionStringBuilder.Port = _appSettings.Database.Port;
        connectionStringBuilder.Database = _appSettings.Database.Name;
        connectionStringBuilder.Username = _appSettings.Database.UserName;
        connectionStringBuilder.Password = _appSettings.Database.Password;
        connectionStringBuilder.MaxPoolSize = 90;
        options
            .UseNpgsql(
                connectionStringBuilder.ConnectionString,
                _ => _
                    // Keep version in sync with the one in ./docker-compose.*.yaml
                    .SetPostgresVersion(18, 4)
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery) // https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries#enabling-split-queries-globally
                    .UseNodaTime()
                    // https://www.npgsql.org/efcore/mapping/enum.html
                    .MapEnum<CalorimetricObserver>(ApplicationDbContext.CalorimetricObserverTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<CoatedSide>(ApplicationDbContext.CoatedSideTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<DataKind>(ApplicationDbContext.DataKindTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<Illuminant>(ApplicationDbContext.IlluminantTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<LogicalCombinator>(ApplicationDbContext.LogicalCombinatorTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<OpticalComponentSubtype>(ApplicationDbContext.OpticalComponentSubtypeTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<OpticalComponentType>(ApplicationDbContext.OpticalComponentTypeTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<PublishingState>(ApplicationDbContext.PublishingStateTypeName, _appSettings.Database.SchemaName)
                    .MapEnum<Standardizer>(ApplicationDbContext.StandardizerTypeName, _appSettings.Database.SchemaName)
            )
            .UseSchemaName(_appSettings.Database.SchemaName)
            .UseOpenIddict()
            .UseProjectables()
            .UsePostgreSqlTriggers();
        if (!environment.IsProduction())
        {
            options
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
        if (environment.IsEnvironment(Program.TestEnvironment))
        {
            options.ConfigureWarnings(_ =>
                _.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)
            );
        }
    }

    private void ConfigureDatabaseServices(IServiceCollection services)
    {
        // Configure the database-context options only once in
        // `AddPooledDbContextFactory` and not a second time in `AddDbContext`
        // as suggested in
        // https://github.com/npgsql/efcore.pg/issues/3375#issuecomment-2509746639
        services.AddPooledDbContextFactory<ApplicationDbContext>(ConfigureDatabaseContext);
        // Database context as services are used by `OpenIddict`, see in
        // particular `AuthConfiguration`.
        services.AddDbContext<ApplicationDbContext>(options =>
            { },
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Singleton
        );
        // services.ConfigureDbContext<ApplicationDbContext>(
        //     ConfigureDatabaseContext,
        //     optionsLifetime: ServiceLifetime.Singleton
        // );
    }

    private void ConfigureHttpClientServices(IServiceCollection services)
    {
        services.AddHttpClient();
        var httpClientBuilder = services.AddHttpClient(ApiRequestService.CustomHttpClient);
        if (environment.IsDevelopment())
        {
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(_ =>
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            );
        }
    }

    public static void ConfigureCustomServices(IServiceCollection services)
    {
        services.AddScoped<AccessPolicyService>();
        services.AddScoped<ApiRequestService>();
        services.AddScoped<ResponseApprovalService>();
        services.AddScoped<UserService>();
        services.AddSingleton<CacheService>();
        services.AddSingleton<MethodFactory>();
        services.AddSingleton<SigningService>();
        services.AddSingleton<FileManager>();
    }

    public static void ConfigureApiRequests(IServiceCollection services)
    {
        services.AddScoped<GetUserInfo>();
        services.AddScoped<IsGnuPgFingerprintValid>();
        services.AddScoped<QueryCurrentUserOrApplication>();
        services.AddScoped<QueryData>();
        services.AddScoped<UpdateDatabase>();
    }

    public void Configure(WebApplication app)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/
        if (environment.IsDevelopment() || environment.IsEnvironment(Program.TestEnvironment))
        {
            app.UseDeveloperExceptionPage();
            // app.UseMigrationsEndPoint();
            // Forwarded Headers Middleware must run before other middleware except diagnostics and error handling middleware. In particular before HSTS middleware.
            // See https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#other-proxy-server-and-load-balancer-scenarios
            app.UseForwardedHeaders();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // Forwarded Headers Middleware must run before other middleware except diagnostics and error handling middleware. In particular before HSTS middleware.
            // See https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#other-proxy-server-and-load-balancer-scenarios
            app.UseForwardedHeaders();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // ASP.NET advices to not use HSTS for APIs, see the warning on
            // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl
            // app.UseHsts(); // Done by the reverse proxy, see https://www.nginx.com/blog/http-strict-transport-security-hsts-and-nginx/
        }

        app.UseStatusCodePagesWithReExecute("/error"); // [UseStatusCodePages](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-9.0#usestatuscodepages)
        // app.UseHttpsRedirection(); // Done by the reverse proxy
        app.UseSerilogRequestLogging();
        app.UseStaticFiles();
        app.UseCookiePolicy(); // [SameSite cookies](https://learn.microsoft.com/en-us/aspnet/core/security/samesite)
        app.UseRouting();
        // [Localization](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)
        app.UseRequestLocalization(_ =>
        {
            _.AddSupportedCultures("en-US");
            _.AddSupportedUICultures("en-US");
            _.SetDefaultCulture("en-US");
        });
        app.UseCors();
        // app.UseCertificateForwarding(); // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-5.0#other-web-proxies
        app.UseWhen(context => context.Request.Path.StartsWithSegments("/connect"), appBuilder =>
        {
            app.UseAuthentication();
        });
        app.UseAuthorization();
        app.UseAntiforgery();
        // app.UseSession(); // Not used
        // app.UseResponseCompression(); // Done by the reverse proxy
        // app.UseResponseCaching(); // Done by the reverse proxy
        // app.UseWebSockets();
        app.MapOpenApi(OpenApiConstants.RoutePattern);
        app.MapScalarApiReference(OpenApiConstants.DocsRoute, _ =>
        {
            _.Servers = []; // https://github.com/dotnet/aspnetcore/issues/57332#issuecomment-2480939916
            _.AddDocument(OpenApiConstants.DocumentName); // For multiple documents see https://guides.scalar.com/scalar/scalar-api-references/integrations/net-aspnet-core/integration#configuration-options__multiple-openapi-documents
            _.WithOpenApiRoutePattern(OpenApiConstants.RoutePattern);
        });
        app.MapGraphQL()
            .RequireCors(GraphQlConstants.CorsPolicy);
        app.MapControllers();
        app.MapHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = WriteJsonResponse
            }
        )
            .DisableHttpMetrics()
            .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
            {
                operation.Description = "Checks whether the webserver is healthy.";
                var responseContent = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema { Type = JsonSchemaType.Object }
                    }
                };
                operation.Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Healthy",
                        Content = responseContent
                    },
                    ["503"] = new OpenApiResponse
                    {
                        Description = "Unhealthy",
                        Content = responseContent
                    }
                };
                return Task.CompletedTask;
            });
    }

    // Inspired by https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0#customize-output
    private static Task WriteJsonResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        var options = new JsonWriterOptions { Indented = true };
        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteString("duration", healthReport.TotalDuration.ToString());
            jsonWriter.WriteStartObject("results");
            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                jsonWriter.WriteString("duration",
                    healthReportEntry.Value.Duration.ToString());
                jsonWriter.WriteStartArray("tags");
                foreach (var tag in healthReportEntry.Value.Tags)
                {
                    jsonWriter.WriteStringValue(tag);
                }

                jsonWriter.WriteEndArray();
                var exception = healthReportEntry.Value.Exception;
                if (exception is not null)
                {
                    jsonWriter.WriteStartObject("exception");
                    jsonWriter.WriteString("message", exception.Message);
                    if (exception.StackTrace is not null)
                    {
                        jsonWriter.WriteString("stackTrace", exception.StackTrace);
                    }

                    if (exception.InnerException is not null)
                    {
                        jsonWriter.WriteString("innerException", exception.InnerException.ToString());
                    }

                    if (exception.Source is not null)
                    {
                        jsonWriter.WriteString("source", exception.Source);
                    }

                    if (exception.TargetSite is not null)
                    {
                        jsonWriter.WriteString("targetSite", exception.TargetSite.ToString());
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteStartObject("data");
                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);
                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}