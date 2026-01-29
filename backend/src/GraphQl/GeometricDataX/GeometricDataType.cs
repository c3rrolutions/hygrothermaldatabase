using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Types;

namespace Database.GraphQl.GeometricDataX;

public sealed class GeometricDataType
    : DataTypeBase<GeometricData, GeometricDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<GeometricData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}