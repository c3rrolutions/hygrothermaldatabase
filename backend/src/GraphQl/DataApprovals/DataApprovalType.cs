using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Extensions;
using Database.GraphQl.References;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.DataApprovals;

public sealed class DataApprovalType
    : ObjectType<DataApproval>
{
    protected override void Configure(IObjectTypeDescriptor<DataApproval> descriptor)
    {
        descriptor
            .Field(t => t.Statement)
            .Type<NonNullType<ReferenceType>>()
            .Resolve(context => context
                .Parent<DataApproval>()
                .Statement
                .TheReference
            );
        descriptor
            .Field(nameof(DataApproval.ApproverId)[..^2].FirstCharToLower())
            .Type<ObjectType<InstitutionDataLoader.Institution>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetApproverAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<InstitutionDataLoader.Institution?> GetApproverAsync(
            [Parent] DataApproval parent,
            IInstitutionByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.ApproverId);
        }
    }
}