using Database.Data;
using HotChocolate.Types;

namespace Database.GraphQl.Approvals;

public sealed class DataApprovalType
    : ObjectType<DataApproval>
{
    protected override void Configure(IObjectTypeDescriptor<DataApproval> descriptor)
    {
        descriptor
            .Field(t => t.Standard)
            .Ignore();
        descriptor
            .Field(t => t.Publication)
            .Ignore();
    }
}