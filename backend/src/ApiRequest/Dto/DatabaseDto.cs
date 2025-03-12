using System;
using System.Collections.Generic;

namespace Database.GraphQl.Databases;

public sealed record DatabaseDto(

     Guid Uuid,
     string Name,
     string Description,
     Uri Locator,
     DatabaseVerificationState VerificationState,
     string VerificationCode,

     // public Institution? Operator { get; set; }
     bool CanCurrentUserUpdateNode,
     bool CanCurrentUserVerifyNode
);

public sealed record DatabasesConnection
(
    IReadOnlyList<DatabaseEdge>? Edges
);
public sealed record DatabaseEdge(
     DatabaseDto Node
);