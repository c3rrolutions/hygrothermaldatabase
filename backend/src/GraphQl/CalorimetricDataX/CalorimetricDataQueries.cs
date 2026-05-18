using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.DataX;
using Database.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Database.GraphQl.CalorimetricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class CalorimetricDataQueries
: DataQueriesBase<CalorimetricData>
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<CalorimetricDataFilterType>]
    [UseSorting<CalorimetricDataSortType>]
    public Task<IEnumerable<CalorimetricData>> GetAllCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.CalorimetricData,
            locale,
            accessRightsService,
            sorting,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<CalorimetricDataFilterType>]
    [UseSorting<CalorimetricDataSortType>]
    public Task<IEnumerable<CalorimetricData>> GetAllPendingCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.CalorimetricData,
            locale,
            accessRightsService,
            sorting,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<CalorimetricDataFilterType>]
    public Task<bool> HasCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.CalorimetricData,
            locale,
            resolverContext,
            cancellationToken
        );
    }

    public Task<CalorimetricData?> GetCalorimetricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        CalorimetricDataByIdDataLoader byId,
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