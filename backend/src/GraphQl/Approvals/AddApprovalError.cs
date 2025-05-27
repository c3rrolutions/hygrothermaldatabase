using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.Approvals;

public class AddApprovalError(
    AddApprovalErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddApprovalErrorCode>(code, message, path)
{
}