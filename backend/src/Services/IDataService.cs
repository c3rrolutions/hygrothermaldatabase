using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;

namespace Database.Services;

/// <summary>
/// Service to get data from <see cref="ApplicationDbContext"/>
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Get dataset with passed id from all datasets
    /// </summary>
    /// <param name="id">                Id of dataset to get. </param>
    /// <param name="context">           <see cref="ApplicationDbContext"/> </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> Dataset with passed id or null. </returns>
    public Task<IData?> GetDataAsync(Guid id, ApplicationDbContext context, CancellationToken cancellationToken);
}