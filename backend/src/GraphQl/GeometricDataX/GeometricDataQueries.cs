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
        IAccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<GeometricData>();
        IQueryable<GeometricData> filteredData = context.GeometricData.Sort(resolverContext).Filter(resolverContext);

        // Check if there is restricted data
        if (!filteredData.Any(x => x.DataAccess == Enumerations.DataAccessMode.RESTRICTED))
        {
            return filteredData;
        }

        // Apply acces rights on data
        var result = await accessRightsService.ApplyAccessRightsOnData(filteredData.ToList<IData>(), cancellationToken).ConfigureAwait(false);

        // TODO Use `locale`.
        return (IQueryable<GeometricData>)result;
    }

    public async Task<GeometricData?> GetGeometricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        GeometricDataByIdDataLoader byId,
        IAccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        var geometricData = await byId.LoadAsync(
            id,
            cancellationToken
        ).ConfigureAwait(false);

        if (geometricData == null)
        {
            return geometricData;
        }

        var result = await accessRightsService.ApplyAccessRightsOnData(geometricData, cancellationToken).ConfigureAwait(false);
        return result as GeometricData;
    }
}