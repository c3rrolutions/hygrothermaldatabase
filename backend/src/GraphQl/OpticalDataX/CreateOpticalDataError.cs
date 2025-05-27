using System.Collections.Generic;

namespace Database.GraphQl.OpticalDataX;

public sealed class CreateOpticalDataError(
    CreateOpticalDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateOpticalDataErrorCode>(code, message, path)
{
}