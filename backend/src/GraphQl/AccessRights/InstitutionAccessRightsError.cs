using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class InstitutionAccessRightsError(
    InstitutionAccessRightsErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<InstitutionAccessRightsErrorCode>(code, message, path)
{
}