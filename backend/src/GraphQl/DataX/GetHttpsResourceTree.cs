using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.GraphQl.GetHttpsResources;
using GreenDonut;
using GreenDonut.Data;
using HotChocolate.Data;
using HotChocolate.Resolvers;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTree(
    Data.IData data
)
{
    public async Task<GetHttpsResourceTreeRoot> GetRoot(
        IHttpsResourceTreeRootByDataIdDataLoader byId,
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
        IResolverContext resolverContext,
        IHttpsResourceTreeNonRootVerticesByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return (
            (await byId
                .With(resolverContext.GetQueryContext<GetHttpsResource>())
                .LoadAsync(data.Id, cancellationToken)
            ) ?? [])
            .Select(_ =>
                new GetHttpsResourceTreeNonRootVertex(_)
            )
            .ToList()
            .AsReadOnly();
    }
}