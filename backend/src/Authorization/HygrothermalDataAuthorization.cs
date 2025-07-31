using System;
using System.Threading;
using Database.ApiRequests.Dto;

namespace Database.Authorization;

public static class HygrothermalDataAuthorization
{
    public static bool IsAuthorizedToCreateHygrothermalDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(currentUser, institutionId);
    }
}