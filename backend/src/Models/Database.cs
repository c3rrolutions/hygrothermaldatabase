using System;

namespace Database.Models;

public sealed record Database(
     Guid Uuid,
     string Name,
     string Description,
     Uri Locator,
     DatabaseVerificationState VerificationState,
     string VerificationCode,
     DatabaseOperatorEdge Operator,
     bool CanCurrentUserUpdateNode,
     bool CanCurrentUserVerifyNode
);