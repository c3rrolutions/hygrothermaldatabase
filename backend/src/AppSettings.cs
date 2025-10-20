// Inspired by https://weblog.west-wind.com/posts/2017/dec/12/easy-configuration-binding-in-aspnet-core-revisited

using System;

namespace Database;

public sealed class AppSettings
{
    private const string GraphQlPathSegment = "/graphql/";

    // TODO Consider using [Flurl](https://flurl.dev) to construct URIs. For the pitfalls of
    // using `Uri` as below see the comments to https://stackoverflow.com/questions/372865/path-combine-for-urls/1527643#1527643
    public Uri DatabaseGraphQlEndpoint { get => new(new Uri(Host, UriKind.Absolute), GraphQlPathSegment); }

    public Uri MetabaseGraphQlEndpoint { get => new(new Uri(MetabaseHost, UriKind.Absolute), GraphQlPathSegment); }

    public string Host { get; private set; }
        = "";

    public string MetabaseHost { get; private set; }
        = "";

    public Guid DatabaseId { get; private set; }
    public Guid OperatorId { get; private set; }

    public string VerificationCode { get; private set; }
        = "";

    public OpenIdConnectClientSettings OpenIdConnectClient { get; private set; } = new();

    public GnupgSecretSigningKeySettings GnupgSecretSigningKey { get; private set; } = new();

    public LoggingSettings Logging { get; private set; } = new();

    public JsonWebTokenSettings JsonWebToken { get; private set; } = new();

    public EmailSettings Email { get; private set; } = new();

    public DatabaseSettings Database { get; private set; } = new();

    public sealed class OpenIdConnectClientSettings
    {
        public string Id { get; private set; } = "";
        public string Secret { get; private set; } = "";
    }

    public sealed class GnupgSecretSigningKeySettings
    {
        // Keep file name {FINGERPRINT}.gpg in sync with the one in the GNU
        // Make target `gpg` in the `Makefile`
        public string FileName { get => $"{Fingerprint}.gpg"; }
        public string Passphrase { get; private set; } = "";
        public string Fingerprint { get; private set; } = "";
    }

    public sealed class LoggingSettings
    {
        public bool EnableSensitiveDataLogging { get; private set; }
    }

    public sealed class JsonWebTokenSettings
    {
        public string EncryptionCertificatePassword { get; private set; }
            = "";

        public string SigningCertificatePassword { get; private set; }
            = "";
    }

    public sealed class EmailSettings
    {
        public string SmtpHost { get; private set; }
            = "";

        public int SmtpPort { get; private set; }
    }

    public sealed class DatabaseSettings
    {
        public string ConnectionString { get; set; }
            = "";

        public string SchemaName { get; private set; }
            = "";
    }
}