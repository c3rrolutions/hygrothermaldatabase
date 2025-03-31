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

namespace Database.GraphQl.HygrothermalDataX;

[ExtendObjectType(nameof(Query))]
public sealed class HygrothermalDataQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<HygrothermalData>> GetAllHygrothermalData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IAccessRightsService accessRightsService,
        IResolverContext resolverContext,
        ISortingContext sorting,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<HygrothermalData>();
        IQueryable<HygrothermalData> filteredData = context.HygrothermalData.Sort(resolverContext).Filter(resolverContext);

        // Check if there is restricted data
        if (!filteredData.Any(x => x.DataAccess == Enumerations.DataAccessMode.RESTRICTED))
        {
            return filteredData;
        }

        // Apply acces rights on data
        var result = await accessRightsService.ApplyAccessRightsOnData(filteredData.ToList<IData>(), cancellationToken).ConfigureAwait(false);

        // TODO Use `locale`.
        return (IQueryable<HygrothermalData>)result;
    }

    public async Task<HygrothermalData?> GetHygrothermalDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        HygrothermalDataByIdDataLoader byId,
        IAccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        var hygrothermalData = await byId.LoadAsync(
            id,
            cancellationToken
        );

        if (hygrothermalData == null)
        {
            return hygrothermalData;
        }

        var result = await accessRightsService.ApplyAccessRightsOnData(hygrothermalData, cancellationToken).ConfigureAwait(false);
        return result as HygrothermalData;
    }
}