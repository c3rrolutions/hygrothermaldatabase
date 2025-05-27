using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class UpdateDataAccessRightsError(
    UpdateDataAccessRightsErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateDataAccessRightsErrorCode>(code, message, path)
{
}