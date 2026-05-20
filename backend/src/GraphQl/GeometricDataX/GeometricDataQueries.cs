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

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class GeometricDataQueries
: DataQueriesBase<GeometricData>
{
    [UsePaging]
    [UseFiltering<GeometricDataFilterType>]
    [UseSorting<GeometricDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<GeometricData>> GetAllGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.GeometricData,
            locale,
            accessRightsService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    [UseFiltering<GeometricDataFilterType>]
    [UseSorting<GeometricDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<GeometricData>> GetAllPendingGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.GeometricData,
            locale,
            accessRightsService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<GeometricDataFilterType>]
    public Task<bool> HasGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.GeometricData,
            locale,
            resolverContext,
            cancellationToken
        );
    }

    public Task<GeometricData?> GetGeometricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        GeometricDataByIdDataLoader byId,
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