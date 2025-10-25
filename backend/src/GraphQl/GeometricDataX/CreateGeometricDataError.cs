using System.Collections.Generic;

namespace Database.GraphQl.GeometricDataX;

public sealed record CreateGeometricDataError(
    CreateGeometricDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateGeometricDataErrorCode>(Code, Message, Path);