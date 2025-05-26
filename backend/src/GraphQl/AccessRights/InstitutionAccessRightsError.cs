using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class InstitutionAccessRightsError
    : UserErrorBase<InstitutionAccessRightsErrorCode>
{
    public InstitutionAccessRightsError(
        InstitutionAccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}