using Database.Data;
using HotChocolate.Types;

namespace Database.GraphQl.Publications;

public sealed class PublicationType
    : ObjectType<Publication>
{
    internal static Publication FromInput(PublicationInput input)
    {
        return new Publication(
            input.Title,
            input.Abstract,
            input.Section,
            input.Authors,
            input.Doi,
            input.ArXiv,
            input.Urn,
            input.WebAddress
        );
    }

    protected override void Configure(
        IObjectTypeDescriptor<Publication> descriptor
    )
    {
    }
}