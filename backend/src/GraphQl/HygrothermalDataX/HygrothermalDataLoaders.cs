using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class HygrothermalDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, HygrothermalData>> GetHygrothermalDataByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<HygrothermalData> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.HygrothermalData,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}