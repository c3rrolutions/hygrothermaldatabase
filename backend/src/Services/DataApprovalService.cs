using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Database.Data;

namespace Database.Services;

public sealed class DataApprovalService(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor
)
{
    public Task<bool> IsGnuPgFingerprintValid(
        string fingerprint,
        Guid institutionId,
        DateTime createdAt,
        CancellationToken cancellationToken
    )
    {
        return ApiRequests.IsGnuPgFingerprintValid.Do(
            fingerprint,
            institutionId,
            createdAt,
            appSettings,
            apiRequestService,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }
}