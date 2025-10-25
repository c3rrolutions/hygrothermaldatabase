using System.Collections.Generic;

namespace Database.GraphQl.ResponseApprovals;

public sealed record UpdateResponseApprovalsError(
    UpdateResponseApprovalsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateResponseApprovalsErrorCode>(Code, Message, Path);