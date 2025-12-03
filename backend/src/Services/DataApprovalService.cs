using System;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Services;

public sealed class DataApprovalService(
    AppSettings appSettings,
    ApiRequestService apiRequestService
)
{
    public Task<bool> IsGnuPgFingerprintValid(
        string fingerprint,
        Guid institutionId,
        OffsetDateTime createdAt,
        CancellationToken cancellationToken
    )
    {
        return ApiRequests.IsGnuPgFingerprintValid.Do(
            fingerprint,
            institutionId,
            createdAt,
            appSettings,
            apiRequestService,
            cancellationToken
        );
    }
}