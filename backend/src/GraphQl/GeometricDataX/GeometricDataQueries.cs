using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class GeometricDataQueries
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<GeometricData>> GetAllGeometricData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<GeometricData>();
        var filteredData =
            context.GeometricData.AsNoTracking()
            .Sort(resolverContext)
            .Filter(resolverContext);
        if (!await filteredData.AnyAsync(x => x.DataAccessRights.HasRestrictions, cancellationToken))
        {
            return filteredData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    [UseFiltering(typeof(GeometricData))]
    public Task<bool> GetHasGeometricData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return context.GeometricData.AsNoTracking()
            .Filter(resolverContext)
            .AnyAsync(cancellationToken);
    }

    public async Task<GeometricData?> GetGeometricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        GeometricDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var geometricData = await byId.LoadAsync(
            id,
            cancellationToken
        );
        if (geometricData is null)
        {
            return null;
        }
        if (!geometricData.DataAccessRights.HasRestrictions)
        {
            return geometricData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(geometricData, cancellationToken);
    }
}