using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public sealed class InstitutionAccessRightsError(
    InstitutionAccessRightsErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<InstitutionAccessRightsErrorCode>(code, message, path)
{
}