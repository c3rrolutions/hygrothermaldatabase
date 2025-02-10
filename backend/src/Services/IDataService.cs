using Database.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Database.Services;

public interface IDataService
{
    public IAsyncEnumerable<IData> GetAllData();

    public Task<IData?> GetDataAsync(Guid id, CancellationToken cancellationToken);
}