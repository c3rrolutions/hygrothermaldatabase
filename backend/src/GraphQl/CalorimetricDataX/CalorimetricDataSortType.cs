using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CalorimetricDataSortType
    : DataSortTypeBase<CalorimetricData>
{
    protected override void Configure(
        ISortInputTypeDescriptor<CalorimetricData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(_ => _.Id);
    }
}
