using System;
using System.Threading;
using Database.ApiRequests.Dto;

namespace Database.Authorization;

public static class PhotovoltaicDataAuthorization
{
    public static bool IsAuthorizedToCreatePhotovoltaicDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        CancellationToken cancellationToken
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(currentUser, institutionId, cancellationToken);
    }
}