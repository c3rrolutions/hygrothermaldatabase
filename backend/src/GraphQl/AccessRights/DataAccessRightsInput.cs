using System;
using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public sealed record DataAccessRightsInput
(
    Guid DataId,
    IReadOnlyDictionary<Guid, uint?>? AllowedUserAndQuantity,
    IReadOnlyList<Guid>? AllowedInstitutions,
    IReadOnlyList<string>? AllowedApplications
);