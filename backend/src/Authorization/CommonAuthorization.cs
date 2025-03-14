using System;
using System.Linq;
using System.Threading;
using Database.ApiRequest.Dto;

namespace Database.Authorization;

public static class CommonAuthorization
{
    public static bool IsCurrentUserAtLeastAssistantOfVerifiedInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        CancellationToken cancellationToken
    )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t => t.Node.Uuid == institutionId && (t.Role == InstitutionRepresentativeRole.ASSISTANT || t.Role == InstitutionRepresentativeRole.OWNER));
    }

    public static bool IsAuthorizedToAddApprovalForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        CancellationToken cancellationToken
        )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t => t.Node.Uuid == institutionId && t.DataSigningPermission == DataSigningPermission.GRANTED);
    }
}