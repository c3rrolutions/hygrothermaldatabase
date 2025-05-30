using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.DataApprovals;

public sealed class AddDataApprovalError(
    AddDataApprovalErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<AddDataApprovalErrorCode>(code, message, path)
{
}