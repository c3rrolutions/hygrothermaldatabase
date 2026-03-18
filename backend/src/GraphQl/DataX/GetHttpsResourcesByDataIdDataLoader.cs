using System;
using System.Linq;
using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourcesByDataIdDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : AssociationsByAssociateIdDataLoader<GetHttpsResource>(
        batchScheduler,
        options,
        dbContextFactory,
        (dbContext, ids) =>
                dbContext.GetHttpsResources.AsNoTracking().Where(x =>
                    ids.Contains(
                        x.CalorimetricDataId
                        ?? x.GeometricDataId
                        ?? x.HygrothermalDataId
                        ?? x.LifeCycleDataId
                        ?? x.OpticalDataId
                        ?? x.PhotovoltaicDataId
                        ?? Guid.Empty
                    )
                ),
        x => x.DataId
        )
{
}