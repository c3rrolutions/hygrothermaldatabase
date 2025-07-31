using System;
using System.Threading;
using Database.ApiRequests.Dto;

namespace Database.Authorization;

public static class GeometricDataAuthorization
{
    public static bool IsAuthorizedToCreateGeometricDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(currentUser, institutionId);
    }
}