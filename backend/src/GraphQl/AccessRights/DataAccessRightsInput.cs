using System;
using Database.Data;

namespace Database.GraphQl.AccessRights;

public sealed record DataAccessRightsInput
(
    Guid DataId,
    DataAccessRights DataAccessRights
);