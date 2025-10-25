using System.Collections.Generic;

namespace Database.GraphQl.OpticalDataX;

public sealed record CreateOpticalDataError(
    CreateOpticalDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateOpticalDataErrorCode>(Code, Message, Path);