using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.GeometricDataX;

[SuppressMessage("Naming", "CA1707")]
public enum CreateGeometricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    SIGNING_FAILED
}