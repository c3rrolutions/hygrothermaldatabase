using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.Methods;

[SuppressMessage("Naming", "CA1707")]
public enum CalculateMethodErrorCode
{
    UNKNOWN_METHOD,
    UNKNOWN_DATABASE,
    DATA_QUERY_FAILED
}