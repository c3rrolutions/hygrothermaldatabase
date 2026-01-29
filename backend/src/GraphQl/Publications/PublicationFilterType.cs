using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.Publications;

public sealed class PublicationFilterType
    : FilterInputType<Publication>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<Publication> descriptor
    )
    {
        descriptor.Field(x => x.Exists).Ignore();
    }
}