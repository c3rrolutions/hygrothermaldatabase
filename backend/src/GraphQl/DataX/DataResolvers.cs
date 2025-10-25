using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Database.Data;

namespace Database.GraphQl.DataX;

public sealed class DataResolvers
{
    public async Task<GetHttpsResource[]> GetGetHttpsResources(
        [Parent] IData data,
        GetHttpsResourcesByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return await byId.LoadAsync(data.Id, cancellationToken) ?? [];
    }

    public GetHttpsResourceTree GetGetHttpsResourceTree(
        [Parent] IData data
    )
    {
        return new GetHttpsResourceTree(data);
    }

    public DateTime GetTimestamp()
    {
        return DateTime.UtcNow;
    }
}