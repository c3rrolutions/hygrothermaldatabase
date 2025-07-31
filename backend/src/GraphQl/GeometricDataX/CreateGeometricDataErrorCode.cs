using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.GeometricDataX;

[SuppressMessage("Naming", "CA1707")]
public enum CreateGeometricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    CREATING_RESPONSE_APPROVAL_FAILED
}