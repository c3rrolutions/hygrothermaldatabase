using System;
using System.Linq;
using Database.ApiRequests.Dto;

namespace Database.Authorization;

public static class CommonAuthorization
{
    public static bool IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(
        CurrentUserDto currentUser
        )
    {
        return currentUser.DatabaseOperatingRepresentedInstitutions.TotalCount >= 1;
    }

    public static bool IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(
        CurrentUserDto currentUser,
        Guid institutionId
    )
    {
        return currentUser.RepresentedInstitutions.Edges.Any(t =>
            (
                t.Role is InstitutionRepresentativeRole.ASSISTANT
                || t.Role is InstitutionRepresentativeRole.OWNER
            )
            &&
            (
                t.Node.Uuid == institutionId
                || t.Node.ManagedInstitutions.Edges.Any(t =>
                    t.Node.Uuid == institutionId
                )
            )
        );
    }
}