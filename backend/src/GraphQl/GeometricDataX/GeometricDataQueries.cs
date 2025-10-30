using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.GraphQl.DataX;
using Database.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Database.Authorization;

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class GeometricDataQueries
: DataQueriesBase<GeometricData>
{
    [UsePaging]
    [UseFiltering<GeometricDataFilterType>]
    [UseSorting<GeometricDataSortType>]
    public Task<IQueryable<GeometricData>> GetAllGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.GeometricData,
            locale,
            accessRightsService,
            sorting,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    [UseFiltering<GeometricDataFilterType>]
    [UseSorting<GeometricDataSortType>]
    public Task<IQueryable<GeometricData>> GetAllPendingGeometricDataAsync(
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
            context.GeometricData,
            locale,
            accessRightsService,
            sorting,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<GeometricDataFilterType>]
    public Task<bool> GetHasGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetHasDataAsync(
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