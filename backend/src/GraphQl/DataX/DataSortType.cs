using Database.Data;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.DataX;

public sealed class DataSortType
    : DataSortTypeBase<IData>
{
    protected override void Configure(
        ISortInputTypeDescriptor<IData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(IData)[1..] + GraphQlConstants.SortInputSuffix);
    }
}