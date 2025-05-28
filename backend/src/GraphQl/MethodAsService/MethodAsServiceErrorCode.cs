using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.MethodAsService;

[SuppressMessage("Naming", "CA1707")]
public enum MethodAsServiceErrorCode
{
    UNKNOWN_METHOD,
    UNKNOWN_DATABASE
}