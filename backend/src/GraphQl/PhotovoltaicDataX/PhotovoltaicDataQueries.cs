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

namespace Database.GraphQl.PhotovoltaicDataX;

[ExtendObjectType(nameof(Query))]
public sealed class PhotovoltaicDataQueries
: DataQueriesBase<PhotovoltaicData>
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<PhotovoltaicDataFilterType>]
    [UseSorting<PhotovoltaicDataSortType>]
    public Task<IEnumerable<PhotovoltaicData>> GetAllPhotovoltaicDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.PhotovoltaicData,
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
    [UseFiltering<PhotovoltaicDataFilterType>]
    [UseSorting<PhotovoltaicDataSortType>]
    public Task<IEnumerable<PhotovoltaicData>> GetAllPendingPhotovoltaicDataAsync(
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
            context.PhotovoltaicData,
            locale,
            accessRightsService,
            sorting,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<PhotovoltaicDataFilterType>]
    public Task<bool> HasPhotovoltaicDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.PhotovoltaicData,
            locale,
            resolverContext,
            cancellationToken
        );
    }

    public Task<PhotovoltaicData?> GetPhotovoltaicDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        PhotovoltaicDataByIdDataLoader byId,
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