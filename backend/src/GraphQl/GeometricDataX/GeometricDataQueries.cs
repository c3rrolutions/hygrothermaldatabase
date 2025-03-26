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
    public async Task<QueryAllDataPayload> GetAllGeometricData(
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
        if (!filteredData.Any(x => x.DataAccess == Enumerations.DataAccessMode.RESTRICTED))
        {
            return new QueryAllDataPayload(filteredData.ToList());
        }

        var result = await accessRightsService.ApplyAccessRightsOnData(filteredData.ToList<IData>(), cancellationToken).ConfigureAwait(false);

        var restrictions = new List<IUserError>();
        foreach (var restriction in result.Restrictions)
        {
            restrictions.Add(new QueryError(
                QueryErrorCode.RESTRICTED,
                restriction,
                []));
        }

        // TODO Use `locale`.
        return new QueryAllDataPayload((ICollection<GeometricData>)result.Data, restrictions);
    }

    public async Task<QueryDataPayload> GetGeometricDataAsync(
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
            return new QueryDataPayload(new QueryError(
                QueryErrorCode.NO_ELEMENTS,
                "No elements",
                []));
        }

        var result = await accessRightsService.ApplyAccessRightsOnData(geometricData, cancellationToken).ConfigureAwait(false);
        if (result.Restrictions is not null)
        {
            return new QueryDataPayload(new QueryError(
                QueryErrorCode.RESTRICTED,
                result.Restrictions,
                []));
        }
        return new QueryDataPayload(result.Data as GeometricData);
    }
}