using HotChocolate.Data.Sorting;
using Database.Data;

namespace Database.GraphQl.Publications;

public sealed class PublicationSortType
    : SortInputType<Publication>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Publication> descriptor
    )
    {
        descriptor.Field(x => x.Exists).Ignore();
    }
}