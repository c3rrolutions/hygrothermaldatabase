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
    [UseFiltering(typeof(GeometricDataFilterType))]
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
        // TODO Use `locale`.
        sorting.StabilizeOrder<GeometricData>();
        var filteredData = context.GeometricData.Sort(resolverContext).Filter(resolverContext);

        // Check if there is restricted data
        if (!filteredData.Any(x => x.DataAccessRights.HasRestrictions))
        {
            return filteredData;
        }

        // Apply acces rights on data
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    public async Task<GeometricData?> GetGeometricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        GeometricDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        var geometricData = await byId.LoadAsync(
            id,
            cancellationToken
        );

        if (geometricData is null)
        {
            return geometricData;
        }

        return await accessRightsService.ApplyAccessRightsOnData(geometricData, cancellationToken);
    }
}