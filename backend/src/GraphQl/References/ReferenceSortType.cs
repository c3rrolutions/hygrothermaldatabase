using Database.Data;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.References;

public sealed class ReferenceSortType
    : SortInputType<Reference>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Reference> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(_ => _.Standard);
        descriptor.Field(_ => _.Publication);
    }
}
