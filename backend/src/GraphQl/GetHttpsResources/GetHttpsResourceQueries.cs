using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.Enumerations;
using Database.Authorization;

namespace Database.GraphQl.GetHttpsResources;

[ExtendObjectType(nameof(Query))]
public sealed class GetHttpsResourceQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    public IQueryable<GetHttpsResource> GetGetHttpsResources(
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<GetHttpsResource>();
        return context.GetHttpsResources.AsNoTracking()
            .Where(_ => _.CalorimetricData == null || _.CalorimetricData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.GeometricData == null || _.GeometricData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.HygrothermalData == null || _.HygrothermalData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.OpticalData == null || _.OpticalData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.PhotovoltaicData == null || _.PhotovoltaicData.PublishingState != PublishingState.PENDING);
    }

    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    public async Task<IQueryable<GetHttpsResource>> GetPendingGetHttpsResources(
        ApplicationDbContext context,
        ISortingContext sorting,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            return Enumerable.Empty<GetHttpsResource>().AsQueryable();
        }
        sorting.StabilizeOrder<GetHttpsResource>();
        return context.GetHttpsResources.AsNoTracking()
            .Where(_ => _.CalorimetricData == null || _.CalorimetricData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.GeometricData == null || _.GeometricData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.HygrothermalData == null || _.HygrothermalData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.OpticalData == null || _.OpticalData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.PhotovoltaicData == null || _.PhotovoltaicData.PublishingState == PublishingState.PENDING);
    }

    public Task<GetHttpsResource?> GetGetHttpsResourceAsync(
        Guid id,
        GetHttpsResourceByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId.LoadAsync(
            id,
            cancellationToken
        );
    }
}