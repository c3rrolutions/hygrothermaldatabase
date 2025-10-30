using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl;

[SuppressMessage("Naming", "CA1716")]
public sealed record Error<TErrorCode>(
    TErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: IUserError
where TErrorCode : struct, Enum;