using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Types;

namespace Database.GraphQl.LifeCycleDataX;

public sealed class LifeCycleDataType
    : DataTypeBase<LifeCycleData, LifeCycleDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<LifeCycleData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}