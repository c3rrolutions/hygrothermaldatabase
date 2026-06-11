using System;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Controllers;
using Database.Data;
using Database.GraphQl.CalorimetricDataX;
using Database.GraphQl.GeometricDataX;
using Database.GraphQl.HygrothermalDataX;
using Database.GraphQl.LifeCycleDataX;
using Database.GraphQl.OpticalDataX;
using Database.GraphQl.PhotovoltaicDataX;
using HotChocolate;
using Microsoft.AspNetCore.Routing;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceResolvers
{
    public async Task<IData?> GetData(
        [Parent] GetHttpsResource getHttpsResource,
        ICalorimetricDataByIdDataLoader calorimetricDataById,
        IHygrothermalDataByIdDataLoader hygrothermalDataById,
        ILifeCycleDataByIdDataLoader lifeCycleDataById,
        IOpticalDataByIdDataLoader opticalDataById,
        IPhotovoltaicDataByIdDataLoader photovoltaicDataById,
        IGeometricDataByIdDataLoader geometricDataById,
        CancellationToken cancellationToken
    )
    {
        if (getHttpsResource.CalorimetricDataId is not null)
        {
            return await calorimetricDataById.LoadAsync(
                getHttpsResource.CalorimetricDataId ?? Guid.Empty,
                cancellationToken
            );
        }
        if (getHttpsResource.GeometricDataId is not null)
        {
            return await geometricDataById.LoadAsync(
                getHttpsResource.GeometricDataId ?? Guid.Empty,
                cancellationToken
            );
        }
        if (getHttpsResource.HygrothermalDataId is not null)
        {
            return await hygrothermalDataById.LoadAsync(
                getHttpsResource.HygrothermalDataId ?? Guid.Empty,
                cancellationToken
            );
        }
        if (getHttpsResource.LifeCycleDataId is not null)
        {
            return await lifeCycleDataById.LoadAsync(
                getHttpsResource.LifeCycleDataId ?? Guid.Empty,
                cancellationToken
            );
        }
        if (getHttpsResource.OpticalDataId is not null)
        {
            return await opticalDataById.LoadAsync(
                getHttpsResource.OpticalDataId ?? Guid.Empty,
                cancellationToken
            );
        }
        if (getHttpsResource.PhotovoltaicDataId is not null)
        {
            return await photovoltaicDataById.LoadAsync(
                getHttpsResource.PhotovoltaicDataId ?? Guid.Empty,
                cancellationToken
            );
        }
        return null;
    }

    public Uri GetLocator(
        [Parent] GetHttpsResource getHttpsResource,
        AppSettings appSettings,
        LinkGenerator linkGenerator
    )
    {
        return new UriBuilder(appSettings.Uri)
        {
            Path = linkGenerator.GetPathByAction(
                controller: nameof(GetHttpsResourcesController),
                action: nameof(GetHttpsResourcesController.Get),
                values: getHttpsResource.FileExtension is null
                    ? new
                    {
                        id = getHttpsResource.Id
                    }
                    : new
                    {
                        id = getHttpsResource.Id,
                        extension = getHttpsResource.FileExtension
                    }
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