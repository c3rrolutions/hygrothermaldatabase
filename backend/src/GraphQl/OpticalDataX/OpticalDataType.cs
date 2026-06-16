using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Types;

namespace Database.GraphQl.OpticalDataX;

public sealed class OpticalDataType
    : DataTypeBase<OpticalData, IOpticalDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpticalData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}