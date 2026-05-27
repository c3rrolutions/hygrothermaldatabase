using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.GraphQl.GetHttpsResources;
using GreenDonut;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;

namespace Database.GraphQl.DataX;

public sealed class DataResolvers
{
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    public Task<GetHttpsResource[]> GetHttpsResources(
        [Parent] IData data,
        IResolverContext resolverContext,
        IHttpsResourcesByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId
            .With(resolverContext.GetQueryContext<GetHttpsResource>())
            .LoadRequiredAsync(data.Id, cancellationToken);
    }

    public GetHttpsResourceTree GetHttpsResourceTree(
        [Parent] IData data
    )
    {
        return new GetHttpsResourceTree(data);
    }

    public Task<DatabaseDataLoader.Database?> GetDatabaseAsync(
        [Parent] IData data,
        IDatabaseByIdDataLoader byId,
        AppSettings appSettings
    )
    {
        return byId.LoadAsync(appSettings.DatabaseId);
    }

    public Task<ComponentDataLoader.Component?> GetComponentAsync(
        [Parent] IData data,
        IComponentByIdDataLoader byId
    )
    {
        return byId.LoadAsync(data.ComponentId);
    }

    public Task<InstitutionDataLoader.Institution?> GetInstitutionAsync(
        [Parent] IData data,
        IInstitutionByIdDataLoader byId
    )
    {
        return byId.LoadAsync(data.CreatorId);
    }
}