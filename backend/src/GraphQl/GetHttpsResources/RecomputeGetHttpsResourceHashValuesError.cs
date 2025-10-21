using System.Collections.Generic;

namespace Database.GraphQl.GetHttpsResources;

public sealed class RecomputeGetHttpsResourceHashValuesError(
    RecomputeGetHttpsResourceHashValuesErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<RecomputeGetHttpsResourceHashValuesErrorCode>(code, message, path)
{
}