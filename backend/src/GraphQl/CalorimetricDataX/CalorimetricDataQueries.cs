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

namespace Database.GraphQl.CalorimetricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class CalorimetricDataQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<CalorimetricData>> GetAllCalorimetricData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IAccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<CalorimetricData>();
        IQueryable<CalorimetricData> filteredData = context.CalorimetricData.Sort(resolverContext).Filter(resolverContext);

        // Check if there is restricted data
        if (!filteredData.Any(x => x.DataAccess == Enumerations.DataAccessMode.RESTRICTED))
        {
            return filteredData;
        }

        // Apply acces rights on data
        var result = await accessRightsService.ApplyAccessRightsOnData(filteredData.ToList<IData>(), cancellationToken).ConfigureAwait(false);

        // TODO Use `locale`.
        return (IQueryable<CalorimetricData>)result;
    }

    public async Task<CalorimetricData?> GetCalorimetricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        CalorimetricDataByIdDataLoader byId,
        IAccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        var calorimetricData = await byId.LoadAsync(
            id,
            cancellationToken
        );
        if (calorimetricData == null)
        {
            return calorimetricData;
        }

        var result = await accessRightsService.ApplyAccessRightsOnData(calorimetricData, cancellationToken).ConfigureAwait(false);
        return result as CalorimetricData;
    }
}