using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.MethodAsService;

public sealed class MethodAsServiceError(
    MethodAsServiceErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<MethodAsServiceErrorCode>(code, message, path)
{
}