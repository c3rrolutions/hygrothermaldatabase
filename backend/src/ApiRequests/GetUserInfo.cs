using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

public sealed class GetUserInfo
{
    public static Uri GetEndpoint(AppSettings appSettings) =>
        new UriBuilder(appSettings.MetabaseHostUri) { Path = "/connect/userinfo" }.Uri;

    public sealed record UserInfo(
        Address? Address,
        string Email,
        bool EmailVerified,
        string Name,
        string? PhoneNumber,
        bool PhoneNumberVerified,
        IReadOnlyList<string>? Roles,
        string Sub, // Subject
        string? Website
    );

    public sealed record Address(
        string Formatted
    );

    /// <summary>
    /// Request user info from Metabase.
    /// </summary>
    public static Task<UserInfo> Do(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        CancellationToken cancellationToken)
    {
        return apiRequestService.PerformHttpGetRequest<UserInfo>(
            GetEndpoint(appSettings),
            cancellationToken
        );
    }
}