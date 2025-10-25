using System.Collections.Generic;

namespace Database.GraphQl.ResponseApprovals;

public sealed record CreateResponseApprovalsError(
    CreateResponseApprovalsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateResponseApprovalsErrorCode>(Code, Message, Path);