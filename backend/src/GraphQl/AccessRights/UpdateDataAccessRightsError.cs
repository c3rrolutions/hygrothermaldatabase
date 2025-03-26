using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class UpdateDataAccessRightsError
    : UserErrorBase<UpdateDataAccessRightsErrorCode>
{
    public UpdateDataAccessRightsError(
        UpdateDataAccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    )
        : base(code, message, path)
    {
    }
}