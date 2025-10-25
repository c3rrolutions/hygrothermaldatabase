using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Database.Data;
using Database.GraphQl.CalorimetricDataX;
using Database.GraphQl.HygrothermalDataX;
using Database.GraphQl.OpticalDataX;
using Database.GraphQl.GeometricDataX;
using Database.GraphQl.PhotovoltaicDataX;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceResolvers
{
    public async Task<IData?> GetData(
        [Parent] GetHttpsResource getHttpsResource,
        CalorimetricDataByIdDataLoader calorimetricDataById,
        HygrothermalDataByIdDataLoader hygrothermalDataById,
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
        if (getHttpsResource.HygrothermalDataId is not null)
        {
            return await hygrothermalDataById.LoadAsync(
                getHttpsResource.HygrothermalDataId ?? Guid.Empty,
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
        if (getHttpsResource.GeometricDataId is not null)
        {
            return await geometricDataById.LoadAsync(
                getHttpsResource.GeometricDataId ?? Guid.Empty,
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
        return new UriBuilder(appSettings.HostUri)
        {
            Path = $"/api/resources/{getHttpsResource.Id}"
        }.Uri;
    }

    public async Task<GetHttpsResource?> GetParent(
        [Parent] GetHttpsResource getHttpsResource,
        GetHttpsResourceByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        // TODO Why is `?? Guid.Empty` below necessary although `getHttpsResource.ParentId` is not null?
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