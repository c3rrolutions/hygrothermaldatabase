using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Data.AccessPolicies;

public sealed class OpenIdConnectApplicationAccessPolicy(
    Guid dataAccessPolicyId,
    string clientId
)
: AccessPolicyBase
{
    [InverseProperty(nameof(DataAccessPolicy.OpenIdConnectApplicationAccessPolicies))]
    public override DataAccessPolicy? DataAccessPolicy { get; set; }

    public Guid DataAccessPolicyId { get; private set; } = dataAccessPolicyId;

    public string ClientId { get; private set; } = clientId;
}