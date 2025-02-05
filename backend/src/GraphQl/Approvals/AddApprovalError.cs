using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.Approvals;

public class AddApprovalError
    : UserErrorBase<AddApprovalErrorCode>
{
    public AddApprovalError(
        AddApprovalErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}