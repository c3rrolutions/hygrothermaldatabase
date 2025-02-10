using System.Threading;
using System.Threading.Tasks;
using Database.Metabase;
using Microsoft.AspNetCore.Http;

namespace Database.Services;

public interface IUserService
{
    public Task<CurrentUser?> GetCurrentUser(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken);
}