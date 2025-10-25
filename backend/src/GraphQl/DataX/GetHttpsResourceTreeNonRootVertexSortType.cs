using HotChocolate.Data.Sorting;
using Database.Data;
using Database.GraphQl.GetHttpsResources;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTreeNonRootVertexSortType
    : GetHttpsResourceSortType
{
    protected override void Configure(
        ISortInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(GetHttpsResourceTreeNonRootVertexSortType)[..^10] + GraphQlConstants.SortInputSuffix);
    }
}