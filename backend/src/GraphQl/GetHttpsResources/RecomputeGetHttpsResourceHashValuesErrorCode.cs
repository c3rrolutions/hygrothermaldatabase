using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.GetHttpsResources;

[SuppressMessage("Naming", "CA1707")]
public enum RecomputeGetHttpsResourceHashValuesErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    FAILED
}