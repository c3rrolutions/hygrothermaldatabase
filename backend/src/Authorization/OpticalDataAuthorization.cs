using System;
using System.Threading;
using Database.ApiRequests.Dto;

namespace Database.Authorization;

public static class OpticalDataAuthorization
{
    public static bool IsAuthorizedToCreateOpticalDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        CancellationToken cancellationToken
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(currentUser, institutionId, cancellationToken);
    }
}