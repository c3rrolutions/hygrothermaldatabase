using System.Collections.Generic;

namespace Database.GraphQl.ResponseApprovals;

public sealed class CreateResponseApprovalsError(
    CreateResponseApprovalsErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateResponseApprovalsErrorCode>(code, message, path)
{
}