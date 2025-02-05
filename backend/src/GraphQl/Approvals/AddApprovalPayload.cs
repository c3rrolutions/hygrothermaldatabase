using Database.Data;

namespace Database.GraphQl.Approvals;

public class AddApprovalPayload
    : ApprovalPayload<AddApprovalError>
{
    public AddApprovalPayload(
        DataApproval dataApproval
    )
        : base(dataApproval)
    {
    }

    public AddApprovalPayload(
        AddApprovalError error
    )
        : base(error)
    {
    }
}