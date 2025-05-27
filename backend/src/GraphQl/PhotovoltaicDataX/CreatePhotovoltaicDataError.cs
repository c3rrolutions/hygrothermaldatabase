using System.Collections.Generic;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class CreatePhotovoltaicDataError(
    CreatePhotovoltaicDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreatePhotovoltaicDataErrorCode>(code, message, path)
{
}