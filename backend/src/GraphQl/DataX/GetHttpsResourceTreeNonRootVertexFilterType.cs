using HotChocolate.Data.Filters;
using Database.Data;
using Database.GraphQl.GetHttpsResources;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTreeNonRootVertexFilterType
    : GetHttpsResourceFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(GetHttpsResourceTreeNonRootVertexFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}