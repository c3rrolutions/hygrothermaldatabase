using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public sealed record UpdateDataAccessRightsError(
    UpdateDataAccessRightsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateDataAccessRightsErrorCode>(Code, Message, Path);