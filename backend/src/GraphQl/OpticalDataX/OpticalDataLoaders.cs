using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.OpticalDataX;

public sealed class OpticalDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, OpticalData>> GetOpticalDataByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<OpticalData> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.OpticalData,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}