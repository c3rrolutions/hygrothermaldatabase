using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.LifeCycleDataX;

public sealed class LifeCycleDataByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<LifeCycleData>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.LifeCycleData
        )
{
}