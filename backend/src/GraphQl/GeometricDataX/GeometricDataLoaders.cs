using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GeometricDataX;

public sealed class GeometricDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, GeometricData>> GetGeometricDataByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<GeometricData> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.GeometricData,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}