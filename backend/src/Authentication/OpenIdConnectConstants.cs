using System;

namespace Database.Authentication;

public static class OpenIdConnectConstants
{
    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(1);

    public const string MetabaseRegistrationId = "metabase";
    public const string MetabaseClientId = "metabase";
    public const string DatabaseQuartzSchedulerId = "database";
    public const string AuthorizationHeaderBearer = "Bearer";
}