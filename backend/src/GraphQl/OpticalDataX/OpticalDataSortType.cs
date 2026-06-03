using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.OpticalDataX;

public sealed class OpticalDataSortType
    : DataSortTypeBase<OpticalData>
{
    protected override void Configure(
        ISortInputTypeDescriptor<OpticalData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(_ => _.Id);
    }
}
