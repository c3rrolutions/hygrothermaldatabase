using System.Collections.Generic;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class CreateHygrothermalDataError(
    CreateHygrothermalDataErrorCode code,
    string message,
    IReadOnlyList<string> path
    )
        : UserErrorBase<CreateHygrothermalDataErrorCode>(code, message, path)
{
}