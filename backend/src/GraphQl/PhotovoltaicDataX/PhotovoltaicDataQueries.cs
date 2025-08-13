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

namespace Database.GraphQl.PhotovoltaicDataX;

[ExtendObjectType(nameof(Query))]
public sealed class PhotovoltaicDataQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<PhotovoltaicData>> GetAllPhotovoltaicData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<PhotovoltaicData>();
        var filteredData = context.PhotovoltaicData
            .Sort(resolverContext)
            .Filter(resolverContext);
        if (!await filteredData.AnyAsync(x => x.DataAccessRights.HasRestrictions, cancellationToken))
        {
            return filteredData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    [UseFiltering(typeof(PhotovoltaicData))]
    public Task<bool> GetHasPhotovoltaicData(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return context.PhotovoltaicData
            .Filter(resolverContext)
            .AnyAsync(cancellationToken);
    }

    public async Task<PhotovoltaicData?> GetPhotovoltaicDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        PhotovoltaicDataByIdDataLoader byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var photovoltaicData = await byId.LoadAsync(
            id,
            cancellationToken
        );
        if (photovoltaicData is null)
        {
            return null;
        }
        if (!photovoltaicData.DataAccessRights.HasRestrictions)
        {
            return photovoltaicData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(photovoltaicData, cancellationToken);
    }
}