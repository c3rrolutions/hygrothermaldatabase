using System;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Controllers;
using Database.Data;
using Database.GraphQl.DataX;
using GreenDonut;
using HotChocolate;
using Microsoft.AspNetCore.Routing;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceResolvers
{
    public Task<IData> GetData(
        [Parent] GetHttpsResource getHttpsResource,
        IDataByIdAndKindDataLoader dataByIdAndKindDataLoader,
        CancellationToken cancellationToken
    )
    {
        return dataByIdAndKindDataLoader.LoadRequiredAsync(
            (getHttpsResource.DataId, getHttpsResource.DataKind),
            cancellationToken
        );
    }

    public Uri GetLocator(
        [Parent] GetHttpsResource getHttpsResource,
        AppSettings appSettings,
        LinkGenerator linkGenerator
    )
    {
        return new UriBuilder(appSettings.Uri)
        {
            Path = linkGenerator.GetPathByRouteValues(
                routeName: GetHttpsResourcesController.ConstructGetActionRouteName(getHttpsResource),
                values: GetHttpsResourcesController.CreateGetActionRouteValues(getHttpsResource)
            )
        }.Uri;
    }

    public async Task<GetHttpsResource?> GetParent(
        [Parent] GetHttpsResource getHttpsResource,
        IGetHttpsResourceByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return getHttpsResource.ParentId is null
            ? null
            : await byId.LoadAsync(getHttpsResource.ParentId ?? Guid.Empty, cancellationToken)!;
    }

    public Task<GetHttpsResource[]> GetChildren(
        [Parent] GetHttpsResource getHttpsResource,
        IHttpsResourceChildrenByGetHttpsResourceIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId.LoadAsync(getHttpsResource.Id, cancellationToken)!;
    }

    public Task<DataFormatDataLoader.DataFormat?> GetDataFormatAsync(
        [Parent] GetHttpsResource getHttpsResource,
        IDataFormatByIdDataLoader byId
    )
    {
        return byId.LoadAsync(getHttpsResource.DataFormatId);
    }
}