using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.AccessPolicies;

public sealed class UserAccessPolicyFilterType
    : AuditableEntityFilterType<UserAccessPolicy>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<UserAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(UserAccessPolicyFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.UserId);
        descriptor.Field(_ => _.IsAlwaysAllowed);
        descriptor.Field(_ => _.IsWithinAccessLimitInTimeSpan);
        descriptor.Field(_ => _.IsWithinTimeSpan);
        descriptor.Field(_ => _.DataAccessPolicy);
        descriptor.Field(_ => _.UpperAccessLimitPerTimeDuration);
        descriptor.Field(_ => _.AccessCountSinceStartTime);
    }
}