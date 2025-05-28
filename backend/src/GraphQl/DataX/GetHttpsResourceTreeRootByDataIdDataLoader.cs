using System;
using System.Linq;
using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTreeRootByDataIdDataLoader(
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
                    x.ParentId == null && ids.Contains(x.CalorimetricDataId ??
                                                       x.HygrothermalDataId ?? x.OpticalDataId ??
                                                       x.PhotovoltaicDataId ?? x.GeometricDataId ?? Guid.Empty)
                ),
        x => x.DataId
        )
{
}