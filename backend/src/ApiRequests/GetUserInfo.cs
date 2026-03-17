using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;

namespace Database.ApiRequests;

public sealed class GetUserInfo(
    AppSettings appSettings,
    ApiRequestService apiRequestService
)
{
    public Uri GetEndpoint =>
        new UriBuilder(appSettings.MetabaseHostUri) { Path = "/connect/userinfo" }.Uri;

    public sealed record UserInfo(
        Address? Address,
        string? Email,
        bool? EmailVerified,
        string Name, // is not null because the OpenId Connect client asks for the scope `Profile`
        string? PhoneNumber,
        bool? PhoneNumberVerified,
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
    public Task<UserInfo> Do(
        CancellationToken cancellationToken)
    {
        return apiRequestService.PerformHttpGetRequest<UserInfo>(
            GetEndpoint,
            cancellationToken
        );
    }
}