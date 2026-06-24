using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate;
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
        descriptor
            .Field(nameof(InstitutionAccessPolicy.InstitutionId)[..^2].FirstCharToLower())
            .Type<ObjectType<InstitutionDataLoader.Institution>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetInstitutionAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<InstitutionDataLoader.Institution?> GetInstitutionAsync(
            [Parent] InstitutionAccessPolicy parent,
            IInstitutionByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.InstitutionId);
        }
    }
}