using System;

namespace Database.GraphQl.AccessRights;

public sealed record AccessRightsInput
(
    Guid InstitutionId,
    int AllowedUserCount,
    int AllowedUserPerTimeSpan,
    TimeSpan Period
);