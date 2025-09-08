using System.Collections.Generic;

namespace Database.GraphQl.Methods;

public sealed class CalculateMethodError(
    CalculateMethodErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CalculateMethodErrorCode>(code, message, path)
{
}