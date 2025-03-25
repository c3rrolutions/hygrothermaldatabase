using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.PhotovoltaicDataX;

[SuppressMessage("Naming", "CA1707")]
public enum CreatePhotovoltaicDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    SIGNING_FAILED
}