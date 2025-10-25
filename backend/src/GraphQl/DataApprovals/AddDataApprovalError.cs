using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.DataApprovals;

public sealed record AddDataApprovalError(
    AddDataApprovalErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<AddDataApprovalErrorCode>(Code, Message, Path);