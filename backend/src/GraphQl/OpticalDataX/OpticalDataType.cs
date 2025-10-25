using HotChocolate.Types;
using Database.Data;
using Database.GraphQl.DataX;

namespace Database.GraphQl.OpticalDataX;

public sealed class OpticalDataType
    : DataTypeBase<OpticalData, OpticalDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpticalData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}