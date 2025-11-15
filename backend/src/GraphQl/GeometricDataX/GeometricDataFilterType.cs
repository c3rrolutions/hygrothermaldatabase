using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.GeometricDataX;

public sealed class GeometricDataFilterType
    : DataFilterTypeBase<GeometricData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<GeometricData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(x => x.Thicknesses);
    }
}