using Database.Data.AccessPolicies;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class UserAccessPolicyType
    : AccessPolicyTypeBase<UserAccessPolicy, IUserAccessPolicyByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<UserAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
    }
}