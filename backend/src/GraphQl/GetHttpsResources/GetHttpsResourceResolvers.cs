using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.GraphQl.CalorimetricDataX;
using Database.GraphQl.GeometricDataX;
using Database.GraphQl.HygrothermalDataX;
using Database.GraphQl.LifeCycleDataX;
using Database.GraphQl.OpticalDataX;
using Database.GraphQl.PhotovoltaicDataX;
using HotChocolate;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceResolvers
{
    public async Task<IData?> GetData(
        [Parent] GetHttpsResource getHttpsResource,
        CalorimetricDataByIdDataLoader calorimetricDataById,
        HygrothermalDataByIdDataLoader hygrothermalDataById,
        LifeCycleDataByIdDataLoader lifeCycleDataById,
        OpticalDataByIdDataLoader opticalDataById,
        PhotovoltaicDataByIdDataLoader photovoltaicDataById,
        GeometricDataByIdDataLoader geometricDataById,
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
        AppSettings appSettings
    )
    {
        return new UriBuilder(appSettings.Uri)
        {
            Path = $"/api/resources/{getHttpsResource.FileName}"
        }.Uri;
    }

    public async Task<GetHttpsResource?> GetParent(
        [Parent] GetHttpsResource getHttpsResource,
        GetHttpsResourceByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return getHttpsResource.ParentId is null
            ? null
            : await byId.LoadAsync(getHttpsResource.ParentId ?? Guid.Empty, cancellationToken)!;
    }

    public Task<GetHttpsResource[]> GetChildren(
        [Parent] GetHttpsResource getHttpsResource,
        GetHttpsResourceChildrenByGetHttpsResourceIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId.LoadAsync(getHttpsResource.Id, cancellationToken)!;
    }
}