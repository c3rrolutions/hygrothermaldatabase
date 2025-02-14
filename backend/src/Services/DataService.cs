using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;

namespace Database.Services;

public class DataService() : IDataService
{
    public async Task<IData?> GetDataAsync(Guid id, ApplicationDbContext context, CancellationToken cancellationToken)
    {
        return context.CalorimetricData.FirstOrDefault(x => x.Id == id) ??
            context.HygrothermalData.FirstOrDefault(x => x.Id == id) ??
            context.OpticalData.FirstOrDefault(x => x.Id == id) ??
            context.GeometricData.FirstOrDefault(x => x.Id == id) ??
            context.PhotovoltaicData.FirstOrDefault(x => x.Id == id) as IData;
    }
}