using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.DataX;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Database.GraphQl.LifeCycleDataX;

[ExtendObjectType(nameof(Query))]
public sealed class LifeCycleDataQueries
: DataQueriesBase<LifeCycleData>
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<LifeCycleDataFilterType>]
    [UseSorting<LifeCycleDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<LifeCycleData>> GetAllLifeCycleDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.LifeCycleData,
            locale,
            accessRightsService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<LifeCycleDataFilterType>]
    [UseSorting<LifeCycleDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<LifeCycleData>> GetAllPendingLifeCycleDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.LifeCycleData,
            locale,
            accessRightsService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<LifeCycleDataFilterType>]
    public Task<bool> HasLifeCycleDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.LifeCycleData,
            locale,
            resolverContext,
            cancellationToken
        );
    }

    public Task<LifeCycleData?> GetLifeCycleDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        LifeCycleDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        return GetDataAsync(
            id,
            locale,
            byId,
            accessRightsService,
            cancellationToken
        );
    }
}