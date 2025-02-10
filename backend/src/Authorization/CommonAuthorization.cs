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
                t.Role == InstitutionRepresentativeRole.ASSISTANT
                || t.Role == InstitutionRepresentativeRole.OWNER
               )
            );
    }

    public static bool IsAuthorizedToAddApprovalForInstitution(
        CurrentUser currentUser,
        Guid institutionId,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t => t.Node.Uuid == institutionId && t.DataSigningPermission == DataSigningPermission.GRANTED);
    }
}