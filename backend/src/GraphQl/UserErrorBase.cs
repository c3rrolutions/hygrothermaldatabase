using System;
using System.Collections.Generic;

namespace Database.GraphQl;

public abstract record UserErrorBase<TUserErrorCode>(
    TUserErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: IUserError
where TUserErrorCode : struct, Enum;