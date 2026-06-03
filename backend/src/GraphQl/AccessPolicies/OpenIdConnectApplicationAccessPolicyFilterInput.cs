using Database.Data.AccessPolicies;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.AccessPolicies;

public sealed class OpenIdConnectApplicationAccessPolicyFilterType
    : FilterInputType<OpenIdConnectApplicationAccessPolicy>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<OpenIdConnectApplicationAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(OpenIdConnectApplicationAccessPolicyFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.ClientId);
    }
}