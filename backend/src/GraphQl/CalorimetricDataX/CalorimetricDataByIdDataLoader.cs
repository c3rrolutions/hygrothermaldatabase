using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CalorimetricDataByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<CalorimetricData>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.CalorimetricData
        )
{
}