using Database.Data;
using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class NamedMethodArgumentType
    : ObjectType<NamedMethodArgument>
{
    protected override void Configure(
        IObjectTypeDescriptor<NamedMethodArgument> descriptor
    )
    {
    }
}