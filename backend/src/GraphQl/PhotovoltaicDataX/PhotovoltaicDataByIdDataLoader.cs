using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class PhotovoltaicDataByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<PhotovoltaicData>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.PhotovoltaicData
        )
{
}