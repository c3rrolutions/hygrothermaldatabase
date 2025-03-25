using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class AccessRightsError
    : UserErrorBase<AccessRightsErrorCode>
{
    public AccessRightsError(
        AccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}