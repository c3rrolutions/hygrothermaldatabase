using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class HygrothermalDataSortType
    : DataSortTypeBase<HygrothermalData>
{
    protected override void Configure(
        ISortInputTypeDescriptor<HygrothermalData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(_ => _.Id);
    }
}
