using Database.Data;
using Database.GraphQl.GetHttpsResources;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTreeNonRootVertexFilterType
    : GetHttpsResourceFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(GetHttpsResourceTreeNonRootVertexFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.CalorimetricData).Ignore();
        descriptor.Field(_ => _.GeometricData).Ignore();
        descriptor.Field(_ => _.HygrothermalData).Ignore();
        descriptor.Field(_ => _.LifeCycleData).Ignore();
        descriptor.Field(_ => _.OpticalData).Ignore();
        descriptor.Field(_ => _.PhotovoltaicData).Ignore();
    }
}
