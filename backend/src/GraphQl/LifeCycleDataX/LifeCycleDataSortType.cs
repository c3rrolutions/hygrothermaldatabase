using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.LifeCycleDataX;

public sealed class LifeCycleDataSortType
    : DataSortTypeBase<LifeCycleData>
{
    protected override void Configure(
        ISortInputTypeDescriptor<LifeCycleData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(x => x.Id);
    }
}