using Database.Data.AccessPolicies;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class InstitutionAccessPolicyType
    : AccessPolicyTypeBase<InstitutionAccessPolicy, IInstitutionAccessPolicyByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<InstitutionAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
    }
}