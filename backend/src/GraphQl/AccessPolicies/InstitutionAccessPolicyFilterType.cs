using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.AccessPolicies;

public sealed class InstitutionAccessPolicyFilterType
    : AuditableEntityFilterType<InstitutionAccessPolicy>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<InstitutionAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(InstitutionAccessPolicyFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.InstitutionId);
        descriptor.Field(_ => _.DataAccessPolicy);
        descriptor.Field(_ => _.UpperAccessLimitPerTimeDuration);
        descriptor.Field(_ => _.AccessCountSinceStartTime);
    }
}