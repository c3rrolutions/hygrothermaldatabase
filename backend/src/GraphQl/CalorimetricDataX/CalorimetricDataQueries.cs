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
    [UseFiltering<CalorimetricDataFilterType>]
    [UseSorting<CalorimetricDataSortType>]
    public async Task<IQueryable<CalorimetricData>> GetAllCalorimetricData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<CalorimetricData>();
        var filteredData =
            context.CalorimetricData.AsNoTracking()
            .Sort(resolverContext)
            .Filter(resolverContext);
        if (!await filteredData.AnyAsync(x => x.DataAccessRights.HasRestrictions, cancellationToken))
        {
            return filteredData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    [UseFiltering<CalorimetricDataFilterType>]
    public Task<bool> GetHasCalorimetricData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return context.CalorimetricData.AsNoTracking()
            .Filter(resolverContext)
            .AnyAsync(cancellationToken);
    }

    public async Task<CalorimetricData?> GetCalorimetricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        CalorimetricDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var calorimetricData = await byId.LoadAsync(
            id,
            cancellationToken
        );
        if (calorimetricData is null)
        {
            return null;
        }
        if (!calorimetricData.DataAccessRights.HasRestrictions)
        {
            return calorimetricData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(calorimetricData, cancellationToken);
    }
}