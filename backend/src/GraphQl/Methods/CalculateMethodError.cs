using System.Collections.Generic;

namespace Database.GraphQl.Methods;

public sealed record CalculateMethodError(
    CalculateMethodErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CalculateMethodErrorCode>(Code, Message, Path);