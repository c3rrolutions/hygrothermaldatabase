using Database.Data;
using HotChocolate.Types;
using Database.GraphQl.DataX;

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