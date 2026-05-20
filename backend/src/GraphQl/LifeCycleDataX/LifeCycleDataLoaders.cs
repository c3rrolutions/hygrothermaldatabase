using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.LifeCycleDataX;

public sealed class LifeCycleDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, LifeCycleData>> GetLifeCycleDataByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<LifeCycleData> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.LifeCycleData,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}