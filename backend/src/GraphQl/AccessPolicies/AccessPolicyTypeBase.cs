using System;
using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using GreenDonut;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public abstract class AccessPolicyTypeBase<TAccessPolicy, TAccessPolicyByIdDataLoader>
    : EntityType<TAccessPolicy, TAccessPolicyByIdDataLoader>
    where TAccessPolicy : AccessPolicyBase
    where TAccessPolicyByIdDataLoader : IDataLoader<Guid, TAccessPolicy>
{
    protected override void Configure(
        IObjectTypeDescriptor<TAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        // descriptor.Field(_ => _.IsAccessAllowed).Ignore();
        // descriptor.Field(_ => _.IsWithinTimeSpan).Ignore();
    }
}