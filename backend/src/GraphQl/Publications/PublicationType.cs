using Database.Data;
using HotChocolate.Types;

namespace Database.GraphQl.Publications;

public sealed class PublicationType
    : ObjectType<Publication>
{
    protected override void Configure(
        IObjectTypeDescriptor<Publication> descriptor
    )
    {
        descriptor.Field(p => p.Exists).Ignore();
    }
}