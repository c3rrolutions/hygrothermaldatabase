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
using NodaTime;

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
    public Task<HotChocolate.Types.Pagination.Connection<PhotovoltaicData>> GetAllPhotovoltaicDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.PhotovoltaicData,
            locale,
            accessRightsService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<PhotovoltaicDataFilterType>]
    [UseSorting<PhotovoltaicDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<PhotovoltaicData>> GetAllPendingPhotovoltaicDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.PhotovoltaicData,
            locale,
            accessRightsService,
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
        IClock clock,
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