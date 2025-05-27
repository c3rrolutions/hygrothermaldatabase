using System.Collections.Generic;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CreateCalorimetricDataError(
    CreateCalorimetricDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateCalorimetricDataErrorCode>(code, message, path)
{
}