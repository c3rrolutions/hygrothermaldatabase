using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.OpticalDataX;

public sealed class OpticalDataByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<OpticalData>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.OpticalData
        )
{
}