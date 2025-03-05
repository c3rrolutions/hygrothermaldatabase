using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class AddDataAccessRightsError
    : UserErrorBase<AddDataAccessRightsErrorCode>
{
    public AddDataAccessRightsError(
        AddDataAccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}