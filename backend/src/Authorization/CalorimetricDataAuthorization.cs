using System;
using System.Threading;
using Database.ApiRequest.Dto;

namespace Database.Authorization;

public static class CalorimetricDataAuthorization
{
    public static bool IsAuthorizedToCreateCalorimetricDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        CancellationToken cancellationToken
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(currentUser, institutionId, cancellationToken);
    }
}