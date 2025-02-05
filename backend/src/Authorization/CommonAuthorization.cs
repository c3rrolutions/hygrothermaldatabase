using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Metabase;
using Microsoft.AspNetCore.Http;

namespace Database.Authorization;

public static class CommonAuthorization
{
    public static async Task<bool> IsCurrentUserAtLeastAssistantOfVerifiedInstitution(
        Guid institutionId,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        return (await QueryingRepresentedInstitutionsByCurrentUser.Query(
            appSettings,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        ).ConfigureAwait(false))
        .Any(
            t =>
            t.Id == institutionId
            && (
                t.Role == QueryingRepresentedInstitutionsByCurrentUser.InstitutionRepresentativeRole.ASSISTANT
                || t.Role == QueryingRepresentedInstitutionsByCurrentUser.InstitutionRepresentativeRole.OWNER
               )
            );
    }

    public static async Task<bool> IsAuthorizedToAddApprovalForInstitution(
        Guid institutionId,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        return (await QueryingRepresentedInstitutionsByCurrentUser.Query(
            appSettings,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        ).ConfigureAwait(false))
        .Any(t =>
            t.Id == institutionId
            && t.Permission == QueryingRepresentedInstitutionsByCurrentUser.DataSigningPermission.GRANTED
            );
    }
}