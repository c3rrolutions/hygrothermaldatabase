using HotChocolate.Types;
using Database.Data;
using Database.GraphQl.References;

namespace Database.GraphQl.DataApprovals;

public sealed class DataApprovalType
    : ObjectType<DataApproval>
{
    protected override void Configure(IObjectTypeDescriptor<DataApproval> descriptor)
    {
        descriptor
            .Field(t => t.Statement)
            .Type<ReferenceType>()
            .Resolve(context => context
                .Parent<DataApproval>()
                .Statement
                .TheReference
            );
    }
}