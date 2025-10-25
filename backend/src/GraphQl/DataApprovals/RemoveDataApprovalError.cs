using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.DataApprovals;

public sealed record RemoveDataApprovalError(
    RemoveDataApprovalErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<RemoveDataApprovalErrorCode>(Code, Message, Path);