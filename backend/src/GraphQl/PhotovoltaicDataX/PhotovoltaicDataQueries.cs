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
        IAccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<PhotovoltaicData>();
        IQueryable<PhotovoltaicData> filteredData = context.PhotovoltaicData.Sort(resolverContext).Filter(resolverContext);

        // Check if there is restricted data
        if (!filteredData.Any(x => x.DataAccess == Enumerations.DataAccessMode.RESTRICTED))
        {
            return filteredData;
        }

        // Apply acces rights on data
        var result = await accessRightsService.ApplyAccessRightsOnData(filteredData.ToList<IData>(), cancellationToken).ConfigureAwait(false);

        // TODO Use `locale`.
        return (IQueryable<PhotovoltaicData>)result;
    }

    public async Task<PhotovoltaicData?> GetPhotovoltaicDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        PhotovoltaicDataByIdDataLoader byId,
        IAccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        var photovoltaicData = await byId.LoadAsync(
            id,
            cancellationToken
        );

        if (photovoltaicData == null)
        {
            return photovoltaicData;
        }

        var result = await accessRightsService.ApplyAccessRightsOnData(photovoltaicData, cancellationToken).ConfigureAwait(false);
        return result as PhotovoltaicData;
    }
}