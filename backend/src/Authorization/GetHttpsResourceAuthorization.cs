using System;
using System.Threading;
using Database.ApiRequest.Dto;

namespace Database.Authorization;

public static class GetHttpsResourceAuthorization
{
    public static bool IsAuthorizedToCreateGetHttpsResourceForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        CancellationToken cancellationToken
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(currentUser, institutionId, cancellationToken);
    }
}