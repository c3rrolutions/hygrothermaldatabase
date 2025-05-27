using Database.GraphQl.GeometricDataX;
using System.Collections.Generic;

namespace Database.GraphQl.MethodAsService;

public class MethodAsServiceError
    : UserErrorBase<MethodAsServiceErrorCode>
{
    public MethodAsServiceError(
        MethodAsServiceErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}