using Microsoft.EntityFrameworkCore;

namespace Database.Data.AccessPolicies;

[PrimaryKey(nameof(ClientId))]
public sealed class OpenIdConnectApplicationAccessPolicy(
    string clientId
)
: AccessPolicyBase
{
    public string ClientId { get; private set; } = clientId;
}