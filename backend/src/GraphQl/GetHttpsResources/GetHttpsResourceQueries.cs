using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.GraphQl.Extensions;
using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GetHttpsResources;

[ExtendObjectType(nameof(Query))]
public sealed class GetHttpsResourceQueries
{
    [UsePaging]
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    public ValueTask<HotChocolate.Types.Pagination.Connection<GetHttpsResource>> GetGetHttpsResources(
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return context.GetHttpsResources.AsNoTracking()
            .Where(_ => _.CalorimetricData == null || _.CalorimetricData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.GeometricData == null || _.GeometricData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.HygrothermalData == null || _.HygrothermalData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.LifeCycleData == null || _.LifeCycleData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.OpticalData == null || _.OpticalData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.PhotovoltaicData == null || _.PhotovoltaicData.PublishingState != PublishingState.PENDING)
            .With(resolverContext.GetQueryContext<GetHttpsResource>(), Sorting.DefaultEntityOrder)
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    [UsePaging]
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async ValueTask<HotChocolate.Types.Pagination.Connection<GetHttpsResource>> GetPendingGetHttpsResources(
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            return await Enumerable.Empty<GetHttpsResource>().AsQueryable()
                .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                .ToConnectionAsync();
        }
        return await context.GetHttpsResources.AsNoTracking()
            .Where(_ => _.CalorimetricData == null || _.CalorimetricData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.GeometricData == null || _.GeometricData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.HygrothermalData == null || _.HygrothermalData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.LifeCycleData == null || _.LifeCycleData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.OpticalData == null || _.OpticalData.PublishingState == PublishingState.PENDING)
            .Where(_ => _.PhotovoltaicData == null || _.PhotovoltaicData.PublishingState == PublishingState.PENDING)
            .With(resolverContext.GetQueryContext<GetHttpsResource>(), Sorting.DefaultEntityOrder)
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    public async Task<GetHttpsResource?> GetGetHttpsResourceAsync(
        Guid id,
        IGetHttpsResourceByIdDataLoader byId,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (await authorization.IsDatabaseOperator(cancellationToken))
        {
            return await byId.LoadAsync(id, cancellationToken);
        }
        return await byId
            .Where(_ => _.CalorimetricData == null || _.CalorimetricData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.GeometricData == null || _.GeometricData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.HygrothermalData == null || _.HygrothermalData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.LifeCycleData == null || _.LifeCycleData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.OpticalData == null || _.OpticalData.PublishingState != PublishingState.PENDING)
            .Where(_ => _.PhotovoltaicData == null || _.PhotovoltaicData.PublishingState != PublishingState.PENDING)
            .LoadAsync(id, cancellationToken);
    }
}