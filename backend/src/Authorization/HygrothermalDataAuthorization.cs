using System;
using System.Net.Http;
using System.Threading;
using Database.ApiRequest.Dto;
using Microsoft.AspNetCore.Http;

namespace Database.Authorization;

public static class HygrothermalDataAuthorization
{
    public static bool IsAuthorizedToCreateHygrothermalDataForInstitution(
        CurrentUserDto currentUser,
        Guid institutionId,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        return CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(currentUser, institutionId, appSettings, httpClientFactory, httpContextAccessor, cancellationToken);
    }
}