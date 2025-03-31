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

namespace Database.GraphQl.OpticalDataX;

[ExtendObjectType(nameof(Query))]
public sealed class OpticalDataQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<OpticalData>> GetAllOpticalData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IAccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<OpticalData>();
        IQueryable<OpticalData> filteredData = context.OpticalData.Sort(resolverContext).Filter(resolverContext);

        // Check if there is restricted data
        if (!filteredData.Any(x => x.DataAccessRights.HasRistrictions))
        {
            return filteredData;
        }

        // Apply acces rights on data
        var result = await accessRightsService.ApplyAccessRightsOnData(filteredData.ToList(), cancellationToken).ConfigureAwait(false);

        // TODO Use `locale`.
        return (IQueryable<OpticalData>)result;
    }

    public async Task<OpticalData?> GetOpticalDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        OpticalDataByIdDataLoader byId,
        IAccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        var opticalData = await byId.LoadAsync(
            id,
            cancellationToken
        );

        if (opticalData == null)
        {
            return opticalData;
        }

        var result = await accessRightsService.ApplyAccessRightsOnData(opticalData, cancellationToken).ConfigureAwait(false);
        return result as OpticalData;
    }
}