using System.Collections.Generic;

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

public enum QueryErrorCode
{
    RESTRICTED
}