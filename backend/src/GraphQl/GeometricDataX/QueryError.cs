using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.GeometricDataX;

public class QueryError
    : UserErrorBase<QueryErrorCode>
{
    public QueryError(
        QueryErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}

[SuppressMessage("Naming", "CA1707")]
public enum QueryErrorCode
{
    NO_ELEMENTS,
    RESTRICTED
}