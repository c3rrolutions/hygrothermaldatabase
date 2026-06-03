using Database.Data;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl;

public sealed class NamedMethodArgumentSortType
    : SortInputType<NamedMethodArgument>
{
    protected override void Configure(
        ISortInputTypeDescriptor<NamedMethodArgument> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(_ => _.Name);
        // TODO descriptor.Field(_ => _.Value);
    }
}
