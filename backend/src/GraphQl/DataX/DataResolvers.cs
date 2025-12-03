using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Database.Data;
using GreenDonut.Data;
using HotChocolate.Data;
using GreenDonut;
using Database.GraphQl.GetHttpsResources;
using HotChocolate.Resolvers;
using Database.GraphQl.Extensions;

namespace Database.GraphQl.DataX;

public sealed class DataResolvers
{
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    public async Task<GetHttpsResource[]> GetGetHttpsResources(
        [Parent] IData data,
        IResolverContext resolverContext,
        GetHttpsResourcesByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        var queryContext = resolverContext.GetQueryContext<GetHttpsResource>();
        return await
            byId
            .With(queryContext)
            .LoadRequiredAsync(data.Id, cancellationToken);
    }

    public GetHttpsResourceTree GetGetHttpsResourceTree(
        [Parent] IData data
    )
    {
        return new GetHttpsResourceTree(data);
    }

    public OffsetDateTime GetTimestamp()
    {
        return OffsetDateTime.UtcNow;
    }
}