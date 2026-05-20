using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Types;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CalorimetricDataType
    : DataTypeBase<CalorimetricData, ICalorimetricDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<CalorimetricData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}