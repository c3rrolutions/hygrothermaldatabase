using System.Collections.Generic;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed record CreatePhotovoltaicDataError(
    CreatePhotovoltaicDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreatePhotovoltaicDataErrorCode>(Code, Message, Path);