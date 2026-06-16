using Database.Data.AccessPolicies;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class UserAccessPolicySortType
    : SortInputType<UserAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<UserAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(UserAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.UserId);
    }
}