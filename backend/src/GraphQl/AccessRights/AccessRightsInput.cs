using System;

namespace Database.GraphQl.AccessRights;

public sealed record AccessRightsInput
(
    // Id of institution
    Guid InstitutionId,
    // Count of allowed user for institution
    // -1 = unlimited 0 = no access
    int AllowedUserCount,
    // Count of allowed datasets for institution
    // -1 = unlimited 0 = no access
    int AllowedDatasetsPerTimeSpan,
    int PeriodInDays
);