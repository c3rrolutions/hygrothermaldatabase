using System;

namespace Database.GraphQl.MethodAsService;

public sealed record MethodAsServiceInput(
    Guid DataId,
    Guid DatabaseId,
    Guid MethodId
);