using OpenIddict.Abstractions;

namespace Database.Authentication;

public static class OpenIdConnectScope
{
    public const string ReadApiScope = "api:read";
    public const string WriteApiScope = "api:write";
    public const string ManageDatabaseApiScope = "api:database:manage";

    public static readonly string[] Scopes =
    [
        OpenIddictConstants.Scopes.Profile,
        ReadApiScope,
        WriteApiScope,
        ManageDatabaseApiScope
    ];
}