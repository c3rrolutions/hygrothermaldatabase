using Database.Data;
using HotChocolate.Types;
using Database.GraphQl.DataX;

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