using System.Collections.Generic;

namespace Database.GraphQl.GetHttpsResources;

public sealed record RecomputeGetHttpsResourceHashValuesError(
    RecomputeGetHttpsResourceHashValuesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<RecomputeGetHttpsResourceHashValuesErrorCode>(Code, Message, Path);