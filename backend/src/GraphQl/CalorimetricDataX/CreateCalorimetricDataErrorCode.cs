using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.CalorimetricDataX;

[SuppressMessage("Naming", "CA1707")]
public enum CreateCalorimetricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    CREATING_RESPONSE_APPROVAL_FAILED
}