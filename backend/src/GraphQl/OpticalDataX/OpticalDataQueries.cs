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
    public Task<IQueryable<OpticalData>> GetAllOpticalData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        sorting.StabilizeOrder<OpticalData>();
        var filteredData = context.OpticalData.Sort(resolverContext).Filter(resolverContext);
        // Check if there is restricted data
        if (!filteredData.Any(x => x.DataAccessRights.HasRestrictions))
        {
            return Task.FromResult(filteredData);
        }
        // Apply acces rights on data
        return accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    public async Task<OpticalData?> GetOpticalDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        OpticalDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
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