using Database.Data;
using HotChocolate.Types;
using Database.GraphQl.DataX;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CalorimetricDataType
    : DataTypeBase<CalorimetricData, CalorimetricDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<CalorimetricData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}