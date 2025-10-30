using Database.Data;
using HotChocolate.Types;

namespace Database.GraphQl.Standards;

public sealed class StandardType
    : ObjectType<Standard>
{
    protected override void Configure(
        IObjectTypeDescriptor<Standard> descriptor
    )
    {
    }
}