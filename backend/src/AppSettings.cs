// Inspired by https://weblog.west-wind.com/posts/2017/dec/12/easy-configuration-binding-in-aspnet-core-revisited

using System;

namespace Database;

public sealed record AppSettings
{
    private const string GraphQlPathSegment = "/graphql/";

    public string Host { private get; init; } = "";
    public string Subdomain { private get; init; } = "";
    public Uri Uri => new($"https://{Subdomain}.{Host}", UriKind.Absolute);
    // TODO Consider using [Flurl](https://flurl.dev) to construct URIs. For the pitfalls of
    // using `Uri` as below see the comments to https://stackoverflow.com/questions/372865/path-combine-for-urls/1527643#1527643
    public Uri DatabaseGraphQlEndpoint => new UriBuilder(Uri) { Path = GraphQlPathSegment }.Uri;

    public string MetabaseHost { get; init; } = "";
    public Uri MetabaseHostUri => new($"https://{MetabaseHost}", UriKind.Absolute);
    public Uri MetabaseGraphQlEndpoint => new UriBuilder(MetabaseHostUri) { Path = GraphQlPathSegment }.Uri;

    public Guid DatabaseId { get; init; }
    public Guid OperatorId { get; init; }

    public string VerificationCode { get; init; } = "";

    public OpenIdConnectClientSettings OpenIdConnectClient { get; init; } = new();
    public GnupgSecretSigningKeySettings GnupgSecretSigningKey { get; init; } = new();
    public LoggingSettings Logging { get; init; } = new();
    public JsonWebTokenSettings JsonWebToken { get; init; } = new();
    public EmailSettings Email { get; init; } = new();
    public DatabaseSettings Database { get; init; } = new();
    public OpenTelemetrySettings OpenTelemetry { get; init; } = new();

    public sealed record OpenIdConnectClientSettings
    {
        public string Id { get; init; } = "";
        public string Secret { get; init; } = "";
    };

    public sealed record GnupgSecretSigningKeySettings
    {
        // Keep file name {FINGERPRINT}.gpg in sync with the one in the GNU
        // Make target `gpg` in the `Makefile`
        public string FileName => $"{Fingerprint}.gpg";
        public string Passphrase { get; init; } = "";
        public string Fingerprint { get; init; } = "";
    };

    public sealed record LoggingSettings
    {
        public bool EnableSensitiveDataLogging { get; init; }
    };

    public sealed record JsonWebTokenSettings
    {
        public string EncryptionCertificatePassword { get; init; } = "";
        public string SigningCertificatePassword { get; init; } = "";
    };

    public sealed record EmailSettings
    {
        public string SmtpHost { get; init; } = "";
        public int SmtpPort { get; init; }
    };

    public sealed record DatabaseSettings
    {
        public string ConnectionString { get; set; } = "";
        public string SchemaName { get; init; } = "";
    };

    public sealed record OpenTelemetrySettings
    {
        public string Host { private get; init; } = "";
        public int GrpcPort { private get; init; }
        public Uri GrpcUri =>
            new UriBuilder(
                scheme: "http",
                host: Host,
                portNumber: GrpcPort
            )
            .Uri;
    };
};