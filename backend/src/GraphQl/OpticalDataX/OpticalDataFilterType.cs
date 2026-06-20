using Database.Data;
using Database.GraphQl.DataX;
using Database.GraphQl.GetHttpsResources;
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
        descriptor.Field(_ => _.Type);
        descriptor.Field(_ => _.Subtype);
        descriptor.Field(_ => _.CoatedSide);
        descriptor.Field(_ => _.NearnormalHemisphericalSolarReflectances);
        descriptor.Field(_ => _.NearnormalHemisphericalSolarTransmittances);
        descriptor.Field(_ => _.NearnormalHemisphericalVisibleReflectances);
        descriptor.Field(_ => _.NearnormalHemisphericalVisibleTransmittances);
        descriptor.Field(_ => _.InfraredEmittances);
        descriptor.Field(_ => _.ColorRenderingIndices);
        descriptor.Field(_ => _.CielabColors);

        // TODO Why are the fields below not included by `base.Configure` above?
        // AuditableEntityFilterType.Configure
        descriptor.Field(_ => _.Id);
        descriptor.Field(_ => _.CreatedAt);
        descriptor.Field(_ => _.UpdatedAt);
        // DataFilterTypeBase.Configure
        descriptor.Field(_ => _.UserId);
        descriptor.Field(_ => _.Locale);
        descriptor.Field(_ => _.Name);
        descriptor.Field(_ => _.Description);
        descriptor.Field(_ => _.ComponentId);
        descriptor.Field(_ => _.CreatorId);
        descriptor.Field(_ => _.AppliedMethod);
        descriptor.Field(_ => _.Approvals);
        descriptor
            .Field(_ => _.Resources)
            .Type<ListFilterInputType<GetHttpsResourceFilterType>>();
        descriptor.Field(_ => _.Warnings);
    }
}