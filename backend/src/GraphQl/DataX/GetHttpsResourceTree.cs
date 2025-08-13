using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTree(
    Data.DataX data
    )
{
    private readonly Data.DataX _data = data;

    public async Task<GetHttpsResourceTreeRoot> GetRoot(
        GetHttpsResourceTreeRootByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return new GetHttpsResourceTreeRoot(
            (await byId.LoadAsync(_data.Id, cancellationToken) ?? []).Single()
        );
    }

    public async Task<IReadOnlyList<GetHttpsResourceTreeNonRootVertex>> GetNonRootVertices(
        GetHttpsResourceTreeNonRootVerticesByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return (await byId.LoadAsync(_data.Id, cancellationToken) ?? [])
            .Select(v =>
                new GetHttpsResourceTreeNonRootVertex(v)
            )
            .ToList()
            .AsReadOnly();
    }
}