using System;

namespace Database.GraphQl.AccessRights;

public sealed record AccessRightsInput
(
    // Id of institution
    Guid InstitutionId,
    // Count of allowed user for institution. Null is unlimited
    uint? AllowedUserCount,
    // Count of allowed datasets for institution. Null is unlimited
    uint? AllowedDatasetsPerTimeSpan,
    int PeriodInDays
);