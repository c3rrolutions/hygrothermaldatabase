using System;
using System.Security.Cryptography.X509Certificates;

namespace Database.Authentication;

public static class OpenIdConnectConstants
{
    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(1);
    public static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(1);

    public const string AuthorizationHeaderBearer = "Bearer";
    public const string DatabaseQuartzSchedulerId = "database";

    public const string MetabaseRegistrationId = "metabase";
    public const string MetabaseClientId = "metabase";

    public const StoreName CertificateStoreName = StoreName.My;
    public const StoreLocation CertificateStoreLocation = StoreLocation.CurrentUser;

    public const string SigningSubjectDistinguishedName = $"CN=Metabase OpenId Connect Client Signing Certificate";
    public const string EncryptionSubjectDistinguishedName = $"CN=Metabase OpenId Connect Client Encryption Certificate";
}