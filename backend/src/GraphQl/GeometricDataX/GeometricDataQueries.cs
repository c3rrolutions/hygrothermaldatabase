using System;
using System.Collections.Generic;
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

        var result = await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken).ConfigureAwait(false);

        var errors = new List<IUserError>();
        foreach (var error in result.Restrictions)
        {
            errors.Add(new QueryError(
                QueryErrorCode.RESTRICTED,
                error,
                []));
        }

        // TODO Use `locale`.
        //return new QueryPayload(result.Data as List<GeometricData>, errors);
        return (result.Data as IEnumerable<GeometricData>).AsQueryable<GeometricData>();
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