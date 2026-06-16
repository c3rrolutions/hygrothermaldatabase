using Database.Data;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.Publications;

public sealed class PublicationSortType
    : SortInputType<Publication>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Publication> descriptor
    )
    {
        descriptor.Field(_ => _.Exists).Ignore();
    }
}
