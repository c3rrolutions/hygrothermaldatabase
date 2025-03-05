using System;
using Database.Data;
using Database.Enumerations;

namespace Database.GraphQl.AccessRights;

public sealed record DataAccessRightsInput
(
    Guid DataId,
    DataAccessMode DataAccessMode,
    DataAccessRights DataAccessRights
);