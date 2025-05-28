using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceByIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : EntityByIdDataLoader<GetHttpsResource>(
        batchScheduler,
        options,
        dbContextFactory,
        dbContext => dbContext.GetHttpsResources
        )
{
}