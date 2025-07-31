using System;
using Database.ApiRequests.Dto;

namespace Database.Authorization;

public static class CalorimetricDataAuthorization
{
    public static bool IsAuthorizedToCreateCalorimetricDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(currentUser, institutionId);
    }
}