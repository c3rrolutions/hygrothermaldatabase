using Database.Data;

namespace Database.GraphQl.DataApprovals;

public sealed class RemoveDataApprovalPayload
    : DataApprovalPayload<RemoveDataApprovalError>
{
    public RemoveDataApprovalPayload(
        DataApproval dataApproval
    )
        : base(dataApproval)
    {
    }

    public RemoveDataApprovalPayload(
        RemoveDataApprovalError error
    )
        : base(error)
    {
    }

    public RemoveDataApprovalPayload(
        DataApproval dataApproval,
        RemoveDataApprovalError error
    )
        : base(dataApproval, error)
    {
    }
}