using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, GetHttpsResource>> GetGetHttpsResourceByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<GetHttpsResource> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.GetHttpsResources,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, GetHttpsResource[]>> GetHttpsResourceChildrenByGetHttpsResourceIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<GetHttpsResource> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetManyByOneIdAsync(
            ids,
            (databaseContext) => databaseContext.GetHttpsResources,
            _ => _.ParentId ?? Guid.Empty,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}