using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.ApiRequests.Dto;
using Microsoft.AspNetCore.Http;

namespace Database.Services;

public sealed class DatabaseService(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory
)
{
    internal Task<DatabaseDto?> QueryDatabase(CancellationToken cancellationToken)
    {
        return DatabaseApi.RequestDatabase(
            appSettings.DatabaseId,
            appSettings,
            apiRequestService,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken);
    }
}