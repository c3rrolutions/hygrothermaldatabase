using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Services;
using Microsoft.EntityFrameworkCore;

namespace Database.Services;

/// <summary>
/// Service to get data from <see cref="ApplicationDbContext"/>
/// </summary>
public sealed class DataService
{
    /// <summary>
    /// Get dataset with passed id from all datasets
    /// </summary>
    /// <param name="id">                Id of dataset to get. </param>
    /// <param name="context">           <see cref="ApplicationDbContext"/> </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> Dataset with passed id or null. </returns>
    public async Task<IData?> GetDataAsync(Guid id, ApplicationDbContext context, CancellationToken cancellationToken)
    {
        return await context.CalorimetricData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ??
            await context.HygrothermalData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ??
            await context.OpticalData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ??
            await context.GeometricData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ??
            await context.PhotovoltaicData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) as IData;
    }
}