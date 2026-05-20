using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

public sealed class DataDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, GetHttpsResource[]>> GetHttpsResourcesByDataIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<GetHttpsResource> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetManyByOneIdAsync(
            ids,
            (databaseContext) => databaseContext.GetHttpsResources,
            _ => _.DataId,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, GetHttpsResource[]>> GetHttpsResourceTreeNonRootVerticesByDataIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<GetHttpsResource> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetManyByOneIdAsync(
            ids,
            (databaseContext) => databaseContext.GetHttpsResources.Where(_ => _.ParentId != null),
            _ => _.DataId,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, GetHttpsResource[]>> GetHttpsResourceTreeRootByDataIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<GetHttpsResource> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetManyByOneIdAsync(
            ids,
            (databaseContext) => databaseContext.GetHttpsResources.Where(_ => _.ParentId == null),
            _ => _.DataId,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}