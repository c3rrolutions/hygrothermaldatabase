using System;
using Database.ApiRequests.Dto;

namespace Database.Authorization;

public static class ResponseApprovalAuthorization
{
    public static bool IsAuthorizedToManageResponseApprovals(
        CurrentUserDto currentUser,
        Guid institutionId
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(currentUser, institutionId);
    }
}