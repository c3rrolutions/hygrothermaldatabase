using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.AccessPolicies;

public class OpenIdConnectApplicationAccessPolicyFilterType
    : AuditableEntityFilterType<OpenIdConnectApplicationAccessPolicy>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<OpenIdConnectApplicationAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(OpenIdConnectApplicationAccessPolicyFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.ClientId);
        descriptor.Field(_ => _.IsAlwaysAllowed);
        descriptor.Field(_ => _.IsWithinAccessLimitInTimeSpan);
        descriptor.Field(_ => _.IsWithinTimeSpan);
        descriptor.Field(_ => _.DataAccessPolicy);
        descriptor.Field(_ => _.UpperAccessLimitPerTimeDuration);
        descriptor.Field(_ => _.AccessCountSinceStartTime);
    }
}