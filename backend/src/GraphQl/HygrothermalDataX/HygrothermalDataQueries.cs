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
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        ISortingContext sorting,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<HygrothermalData>();
        var filteredData =
            context.HygrothermalData.AsNoTracking()
            .Sort(resolverContext)
            .Filter(resolverContext);
        if (!await filteredData.AnyAsync(x => x.DataAccessRights.HasRestrictions, cancellationToken))
        {
            return filteredData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    [UseFiltering(typeof(HygrothermalData))]
    public Task<bool> GetHasHygrothermalData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return context.HygrothermalData.AsNoTracking()
            .Filter(resolverContext)
            .AnyAsync(cancellationToken);
    }

    public async Task<HygrothermalData?> GetHygrothermalDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        HygrothermalDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var hygrothermalData = await byId.LoadAsync(
            id,
            cancellationToken
        );
        if (hygrothermalData is null)
        {
            return null;
        }
        if (!hygrothermalData.DataAccessRights.HasRestrictions)
        {
            return hygrothermalData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(hygrothermalData, cancellationToken);
    }
}