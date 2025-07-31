using System.Collections.Generic;

namespace Database.GraphQl.ResponseApprovals;

public sealed class UpdateResponseApprovalsError(
    UpdateResponseApprovalsErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateResponseApprovalsErrorCode>(code, message, path)
{
}