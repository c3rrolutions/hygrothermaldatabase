using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public sealed record InstitutionAccessRightsError(
    InstitutionAccessRightsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<InstitutionAccessRightsErrorCode>(Code, Message, Path);