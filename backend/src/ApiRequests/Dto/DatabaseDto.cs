using System;
using System.Collections.Generic;
using Database.GraphQl.Databases;

namespace Database.ApiRequests.Dto;

public sealed record Institution(
     Guid Uuid
);

public sealed record DatabaseOperatorEdge(
     Institution Node
);

public sealed record DatabaseDto(

     Guid Uuid,
     string Name,
     string Description,
     Uri Locator,
     DatabaseVerificationState VerificationState,
     string VerificationCode,
     DatabaseOperatorEdge Operator,

     // public Institution? Operator { get; set; }
     bool CanCurrentUserUpdateNode,
     bool CanCurrentUserVerifyNode
);

public sealed record ErrorDto(
     string Code,
     string Message,
     IReadOnlyList<string> Path
);

public sealed record DatabasePayloadDto(
     DatabaseDto? Database,
     IReadOnlyList<ErrorDto>? Errors
);