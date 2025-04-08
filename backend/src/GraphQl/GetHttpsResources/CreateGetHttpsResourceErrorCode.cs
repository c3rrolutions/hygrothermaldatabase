using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.GetHttpsResources;

[SuppressMessage("Naming", "CA1707")]
public enum CreateGetHttpsResourceErrorCode
{
    UNKNOWN,
    UNKNOWN_DATA,
    UNAUTHORIZED,
    UNAUTHENTICATED,
}