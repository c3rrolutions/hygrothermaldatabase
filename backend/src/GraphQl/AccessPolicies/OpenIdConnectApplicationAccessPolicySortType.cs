using Database.Data.AccessPolicies;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class OpenIdConnectApplicationAccessPolicySortType
    : SortInputType<OpenIdConnectApplicationAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<OpenIdConnectApplicationAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(OpenIdConnectApplicationAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.ClientId);
    }
}