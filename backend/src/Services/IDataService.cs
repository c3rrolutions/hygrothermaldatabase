using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;

namespace Database.Services;

public interface IDataService
{
    public Task<IData?> GetDataAsync(Guid id, ApplicationDbContext context, CancellationToken cancellationToken);
}