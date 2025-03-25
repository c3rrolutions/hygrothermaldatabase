using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest.Dto;

namespace Database.Services;

/// <summary>
/// Service to get current user from Metabase
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get current user from Metabase.
    /// </summary>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> </returns>
    public Task<CurrentUserDto?> GetCurrentUser(CancellationToken cancellationToken);

    /// <summary>
    /// Get application from user calims.
    /// </summary>
    /// <returns> ClientId from claims as applicationId. </returns>
    string? GetApplicationIdFromUser();
}