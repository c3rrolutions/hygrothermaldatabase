using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.HygrothermalDataX;

[SuppressMessage("Naming", "CA1707")]
public enum CreateHygrothermalDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    SIGNING_FAILED
}