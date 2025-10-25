using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using HotChocolate.Data;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTree(
    Data.IData data
)
{
    public async Task<GetHttpsResourceTreeRoot> GetRoot(
        GetHttpsResourceTreeRootByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return new GetHttpsResourceTreeRoot(
            (await byId.LoadRequiredAsync(data.Id, cancellationToken))
            .Single()
        );
    }

    [UseFiltering<GetHttpsResourceTreeNonRootVertexFilterType>]
    [UseSorting<GetHttpsResourceTreeNonRootVertexSortType>]
    public async Task<IReadOnlyList<GetHttpsResourceTreeNonRootVertex>> GetNonRootVertices(
        QueryContext<GetHttpsResource> queryContext,
        GetHttpsResourceTreeNonRootVerticesByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return (
            await byId
                .With(queryContext)
                .LoadRequiredAsync(data.Id, cancellationToken)
            )
            .Select(v =>
                new GetHttpsResourceTreeNonRootVertex(v)
            )
            .ToList()
            .AsReadOnly();
    }
}