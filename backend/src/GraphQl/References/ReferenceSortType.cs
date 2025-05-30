using HotChocolate.Data.Sorting;
using Database.Data;

namespace Database.GraphQl.References;

public sealed class ReferenceSortType
    : SortInputType<Reference>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Reference> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Standard);
        descriptor.Field(x => x.Publication);
    }
}