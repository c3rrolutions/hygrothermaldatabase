using System;
using System.Threading;
using Database.ApiRequest.Dto;

namespace Database.Authorization;

public static class GeometricDataAuthorization
{
    public static bool IsAuthorizedToCreateGeometricDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        CancellationToken cancellationToken
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(currentUser, institutionId, cancellationToken);
    }
}