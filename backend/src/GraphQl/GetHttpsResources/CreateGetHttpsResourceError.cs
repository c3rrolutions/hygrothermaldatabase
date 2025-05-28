using System.Collections.Generic;

namespace Database.GraphQl.GetHttpsResources;

public sealed class CreateGetHttpsResourceError(
    CreateGetHttpsResourceErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateGetHttpsResourceErrorCode>(code, message, path)
{
}