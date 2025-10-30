using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Database.Data;
using Database.GraphQl.Extensions;

namespace Database.GraphQl.GetHttpsResources;

[ExtendObjectType(nameof(Query))]
public sealed class GetHttpsResourceQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    public IQueryable<GetHttpsResource> GetGetHttpsResources(
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<GetHttpsResource>();
        return context.GetHttpsResources.AsNoTracking();
    }

    public Task<GetHttpsResource?> GetGetHttpsResourceAsync(
        Guid id,
        GetHttpsResourceByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId.LoadAsync(
            id,
            cancellationToken
        );
    }
}