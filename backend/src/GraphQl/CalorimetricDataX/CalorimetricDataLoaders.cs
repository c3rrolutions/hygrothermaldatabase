using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CalorimetricDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, CalorimetricData>> GetCalorimetricDataByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<CalorimetricData> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.CalorimetricData,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}