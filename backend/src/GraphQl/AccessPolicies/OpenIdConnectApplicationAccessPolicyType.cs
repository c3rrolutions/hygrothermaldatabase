using Database.Data.AccessPolicies;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class OpenIdConnectApplicationAccessPolicyType
    : AccessPolicyTypeBase<OpenIdConnectApplicationAccessPolicy, IOpenIdConnectApplicationAccessPolicyByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIdConnectApplicationAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
    }
}