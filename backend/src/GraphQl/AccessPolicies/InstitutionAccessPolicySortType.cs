using Database.Data.AccessPolicies;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class InstitutionAccessPolicySortType
    : SortInputType<InstitutionAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<InstitutionAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(InstitutionAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.InstitutionId);
    }
}