using Database.Data.AccessPolicies;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.AccessPolicies;

public sealed class UserAccessPolicyFilterType
    : FilterInputType<UserAccessPolicy>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<UserAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(UserAccessPolicyFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.UserId);
    }
}