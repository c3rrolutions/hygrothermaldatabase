using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.GeometricDataX;

public sealed class GeometricDataSortType
    : DataSortTypeBase<GeometricData>
{
    protected override void Configure(
        ISortInputTypeDescriptor<GeometricData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(_ => _.Id);
    }
}
