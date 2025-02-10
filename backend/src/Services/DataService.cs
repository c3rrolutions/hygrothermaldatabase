using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.GraphQl.CalorimetricDataX;
using Database.GraphQl.GeometricDataX;
using Database.GraphQl.HygrothermalDataX;
using Database.GraphQl.OpticalDataX;
using Database.GraphQl.PhotovoltaicDataX;
using Microsoft.EntityFrameworkCore;

namespace Database.Services;

public class DataService(
        ApplicationDbContext context,
        CalorimetricDataByIdDataLoader calorimetricDataById,
        HygrothermalDataByIdDataLoader hygrothermalDataById,
        OpticalDataByIdDataLoader opticalDataById,
        PhotovoltaicDataByIdDataLoader photovoltaicDataById,
        GeometricDataByIdDataLoader geometricDataById) : IDataService
{
    public IAsyncEnumerable<IData> GetAllData()
    {
        // The union below does sadly not work because the different kinds of data have different
        // include operations. return context.CalorimetricData.AsNoTracking<Data.IData>()
        // .Union(context.HygrothermalData.AsNoTracking<Data.IData>())
        // .Union(context.OpticalData.AsNoTracking<Data.IData>()) .Union(context.PhotovoltaicData.AsNoTracking<Data.IData>());
        return context.CalorimetricData.AsNoTracking<IData>().AsAsyncEnumerable()
            .Concat(context.GeometricData.AsNoTracking<IData>().AsAsyncEnumerable())
            .Concat(context.HygrothermalData.AsNoTracking<IData>().AsAsyncEnumerable())
            .Concat(context.OpticalData.AsNoTracking<IData>().AsAsyncEnumerable())
            .Concat(context.PhotovoltaicData.AsNoTracking<IData>().AsAsyncEnumerable());
    }

    public async Task<IData?> GetDataAsync(Guid id, CancellationToken cancellationToken)
    {
        return await calorimetricDataById.LoadAsync(id, cancellationToken).ConfigureAwait(false) ??
            await hygrothermalDataById.LoadAsync(id, cancellationToken).ConfigureAwait(false) ??
            await opticalDataById.LoadAsync(id, cancellationToken).ConfigureAwait(false) ??
            await geometricDataById.LoadAsync(id, cancellationToken).ConfigureAwait(false) ??
            await photovoltaicDataById.LoadAsync(id, cancellationToken).ConfigureAwait(false) as IData;
    }
}