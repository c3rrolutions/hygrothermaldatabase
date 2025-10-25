using System.Collections.Generic;

namespace Database.GraphQl.HygrothermalDataX;

public sealed record CreateHygrothermalDataError(
    CreateHygrothermalDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateHygrothermalDataErrorCode>(Code, Message, Path);