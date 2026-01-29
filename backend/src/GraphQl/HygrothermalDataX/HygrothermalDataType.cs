using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Types;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class HygrothermalDataType
    : DataTypeBase<HygrothermalData, HygrothermalDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<HygrothermalData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}