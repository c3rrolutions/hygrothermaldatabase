using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class AddAccessRightsError
    : UserErrorBase<AddAccessRightsErrorCode>
{
    public AddAccessRightsError(
        AddAccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}