using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.CalorimetricDataX;

[SuppressMessage("Naming", "CA1707")]
public enum CreateCalorimetricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    SIGNING_FAILED
}