using System.Collections.Generic;

namespace Database.GraphQl.Databases;

public sealed class UpdateDatabaseError(
    UpdateDatabaseErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<UpdateDatabaseErrorCode>(code, message, path)
{
}