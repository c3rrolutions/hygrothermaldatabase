using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.DataApprovals;

public sealed class RemoveDataApprovalError(
    RemoveDataApprovalErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RemoveDataApprovalErrorCode>(code, message, path)
{
}