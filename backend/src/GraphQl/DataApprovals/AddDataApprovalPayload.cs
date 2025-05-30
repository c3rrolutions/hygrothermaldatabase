using Database.Data;

namespace Database.GraphQl.DataApprovals;

public sealed class AddDataApprovalPayload
    : DataApprovalPayload<AddDataApprovalError>
{
    public AddDataApprovalPayload(
        DataApproval dataApproval
    )
        : base(dataApproval)
    {
    }

    public AddDataApprovalPayload(
        AddDataApprovalError error
    )
        : base(error)
    {
    }

    public AddDataApprovalPayload(
        DataApproval dataApproval,
        AddDataApprovalError error
    )
        : base(dataApproval, error)
    {
    }
}