using System.Collections.Generic;

namespace Database.GraphQl.GetHttpsResources;

public sealed record CreateGetHttpsResourceError(
    CreateGetHttpsResourceErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateGetHttpsResourceErrorCode>(Code, Message, Path);