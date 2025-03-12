using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest.Dto;
using Microsoft.AspNetCore.Http;

namespace Database.Services;

/// <summary>
/// Service to get current user from Metabase
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get current user from Mewtabase by extraxting token from <see cref="HttpContextAccessor"/>.
    /// </summary>
    /// <param name="httpContextAccessor"> <see cref="HttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> </returns>
    public Task<CurrentUserDto?> GetCurrentUser(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken);
}