using HotChocolate.Types;
using HotChocolate;
using System;

namespace Database.GraphQl.MethodAsService;

public sealed record MethodAsServiceWithFileInput(
    [GraphQLType(typeof(NonNullType<UploadType>))] IFile File,
    Guid MethodId
    );