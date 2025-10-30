using System;
using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class ErrorType<TErrorCode>
    : ObjectType<Error<TErrorCode>>
    where TErrorCode : struct, Enum
{
    protected override void Configure(
        IObjectTypeDescriptor<Error<TErrorCode>> descriptor)
    {
        descriptor
            .Name(dependency => dependency.Name[..^"Code".Length])
            .DependsOn<ObjectType<TErrorCode>>();
        descriptor
            .Field(_ => _.Code)
            .Type<ObjectType<TErrorCode>>();
    }
}