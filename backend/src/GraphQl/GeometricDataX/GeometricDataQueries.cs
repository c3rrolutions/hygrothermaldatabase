using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Database.Data;
using Database.GraphQl.Extensions;
using HotChocolate.Resolvers;

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class GeometricDataQueries
{
    [UsePaging]
    [UseFiltering(typeof(GeometricDataFilterType))]
    [UseSorting]
    public IQueryable<GeometricData> GetAllGeometricData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        ISortingContext sorting,
        IResolverContext resolverContext
    )
    {
        sorting.StabilizeOrder<GeometricData>();
        IQueryable<GeometricData> filteredData = context.GeometricData.Sort(resolverContext).Filter(resolverContext);

        // TODO Use `locale`.
        return filteredData;
    }

    public Task<GeometricData?> GetGeometricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        GeometricDataByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        return byId.LoadAsync(
            id,
            cancellationToken
        );
    }
}