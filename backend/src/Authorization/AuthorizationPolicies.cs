namespace Database.Authorization;

public static class AuthorizationPolicies
{
    public const string AuthenticatedPolicy = "Authenticated";
    public const string ReadScopePolicy = "ReadScope";
    public const string WriteScopePolicy = "WriteScope";
}