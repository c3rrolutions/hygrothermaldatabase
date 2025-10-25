using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.Services;

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
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<OpticalData>();
        var filteredData =
            context.OpticalData.AsNoTracking()
            .Sort(resolverContext)
            .Filter(resolverContext);
        if (!await filteredData.AnyAsync(x => x.DataAccessRights.HasRestrictions, cancellationToken))
        {
            return filteredData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    [UseFiltering(typeof(OpticalData))]
    public Task<bool> GetHasOpticalData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return context.OpticalData.AsNoTracking()
            .Filter(resolverContext)
            .AnyAsync(cancellationToken);
    }

    public async Task<OpticalData?> GetOpticalDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        OpticalDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var opticalData = await byId.LoadAsync(
            id,
            cancellationToken
        );
        if (opticalData is null)
        {
            return null;
        }
        if (!opticalData.DataAccessRights.HasRestrictions)
        {
            return opticalData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(opticalData, cancellationToken);
    }
}