using System;
using System.Collections.Generic;
using Database.Enumerations;

namespace Database.GraphQl.AccessRights;

public sealed record DataAccessRightsInput
(
    Guid DataId,
    DataKind DataKind,
    IReadOnlyDictionary<Guid, uint?>? AllowedUserAndQuantity,
    IReadOnlyList<Guid>? AllowedInstitutions,
    IReadOnlyList<string>? AllowedApplications
);