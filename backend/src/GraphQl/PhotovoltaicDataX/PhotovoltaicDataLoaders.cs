using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class PhotovoltaicDataLoaders
: DataLoaders
{
    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, PhotovoltaicData>> GetPhotovoltaicDataByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<PhotovoltaicData> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.PhotovoltaicData,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}