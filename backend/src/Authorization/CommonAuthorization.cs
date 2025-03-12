using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Database.ApiRequest.Dto;
using Microsoft.AspNetCore.Http;

namespace Database.Authorization;

public static class CommonAuthorization
{
    public static bool IsCurrentUserAtLeastAssistantOfVerifiedInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t => t.Node.Uuid == institutionId && (t.Role == InstitutionRepresentativeRole.ASSISTANT || t.Role == InstitutionRepresentativeRole.OWNER));
    }

    public static bool IsAuthorizedToAddApprovalForInstitution(
        CurrentUserDto currentUser,
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