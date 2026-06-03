using Database.Data.AccessPolicies;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.AccessPolicies;

public sealed class InstitutionAccessPolicyFilterType
    : FilterInputType<InstitutionAccessPolicy>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<InstitutionAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(InstitutionAccessPolicyFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.InstitutionId);
    }
}