using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CalorimetricDataFilterType
    : DataFilterTypeBase<CalorimetricData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<CalorimetricData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(x => x.GValues);
        descriptor.Field(x => x.UValues);
    }
}