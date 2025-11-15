using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class HygrothermalDataFilterType
    : DataFilterTypeBase<HygrothermalData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<HygrothermalData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}