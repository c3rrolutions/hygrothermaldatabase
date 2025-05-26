using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.Services;

/// <summary>
/// Implementation of <see cref="IDataService"/>
/// </summary>
public class DataService() : IDataService
{
    /// <inheritdoc/>
    public async Task<IData?> GetDataAsync(Guid id, ApplicationDbContext context, CancellationToken cancellationToken)
    {
        return await context.CalorimetricData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false) ??
            await context.HygrothermalData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false) ??
            await context.OpticalData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false) ??
            await context.GeometricData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false) ??
            await context.PhotovoltaicData.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false) as IData;
    }
}