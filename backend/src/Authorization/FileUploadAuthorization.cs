using System;
using System.Net.Http;
using System.Threading;
using Database.Metabase;
using Microsoft.AspNetCore.Http;

namespace Database.Authorization;

public static class FileUploadDataAuthorization
{
    public static bool IsAuthorizedToUploadFilesForInstitution(
        CurrentUser currentUser,
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