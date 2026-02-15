using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.OpticalDataX;

public sealed class OpticalDataFilterType
    : DataFilterTypeBase<OpticalData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<OpticalData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(OpticalDataFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(x => x.Type);
        descriptor.Field(x => x.Subtype);
        descriptor.Field(x => x.CoatedSide);
        descriptor.Field(x => x.NearnormalHemisphericalSolarReflectances);
        descriptor.Field(x => x.NearnormalHemisphericalSolarTransmittances);
        descriptor.Field(x => x.NearnormalHemisphericalVisibleReflectances);
        descriptor.Field(x => x.NearnormalHemisphericalVisibleTransmittances);
        descriptor.Field(x => x.InfraredEmittances);
        descriptor.Field(x => x.ColorRenderingIndices);
        descriptor.Field(x => x.CielabColors);
    }
}