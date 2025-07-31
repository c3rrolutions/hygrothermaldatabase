using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.OpticalDataX;

[SuppressMessage("Naming", "CA1707")]
public enum CreateOpticalDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    CREATING_RESPONSE_APPROVAL_FAILED
}