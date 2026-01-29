using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Extensions;
using Database.GraphQl.Extensions;
using Database.GraphQl.GetHttpsResources;
using GreenDonut;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using NodaTime;

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