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
        descriptor.Field(x => x.CalorimetricData).Ignore();
        descriptor.Field(x => x.GeometricData).Ignore();
        descriptor.Field(x => x.HygrothermalData).Ignore();
        descriptor.Field(x => x.LifeCycleData).Ignore();
        descriptor.Field(x => x.OpticalData).Ignore();
        descriptor.Field(x => x.PhotovoltaicData).Ignore();
    }
}