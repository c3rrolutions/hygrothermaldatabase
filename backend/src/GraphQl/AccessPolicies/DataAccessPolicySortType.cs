using Database.Data.AccessPolicies;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class DataAccessPolicySortType
    : SortInputType<DataAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(DataAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.DataId);
        descriptor.Field(_ => _.DataKind);
    }
}