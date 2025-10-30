using HotChocolate.Data.Filters;
using Database.Data;

namespace Database.GraphQl.GetHttpsResources;

public sealed class RecomputeGetHttpsResourceHashValuesFilterType
    : GetHttpsResourceFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(RecomputeGetHttpsResourceHashValuesFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}