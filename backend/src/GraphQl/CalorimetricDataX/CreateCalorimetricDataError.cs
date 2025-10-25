using System.Collections.Generic;

namespace Database.GraphQl.CalorimetricDataX;

public sealed record CreateCalorimetricDataError(
    CreateCalorimetricDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateCalorimetricDataErrorCode>(Code, Message, Path);