using System.Collections.Generic;

namespace Database.GraphQl.GeometricDataX;

public sealed class CreateGeometricDataError(
    CreateGeometricDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateGeometricDataErrorCode>(code, message, path)
{
}