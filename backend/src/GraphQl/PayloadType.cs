using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class PayloadType<TErrorType>
    : ObjectType<Payload<TErrorType>>
{
    protected override void Configure(
        IObjectTypeDescriptor<Payload<TErrorType>> descriptor)
    {
        descriptor
            .Name(dependency => dependency.Name[..^"Error".Length] + nameof(Payload))
            .DependsOn<ObjectType<TErrorType>>();
        descriptor
            .Field(_ => _.Errors)
            .Type<ObjectType<TErrorType>>();
    }
}