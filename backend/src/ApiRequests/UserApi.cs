using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests.Dto;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

/// <summary>
/// Class to request user info from Metabase API.
/// </summary>
public class UserApi
{
    private static readonly string[] _currentUserFileNames =
    {
        "CurrentUser.graphql"
    };

    private sealed record CurrentUserData(CurrentUserDto CurrentUser);

    /// <summary>
    /// Request current user from Metabase.
    /// </summary>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="CurrentUserDto"/> if successful. </returns>
    public static async Task<CurrentUserDto?> RequestCurrentUser(
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        return (await apiRequestService.Metabase().QueryGraphQl<CurrentUserData>(
                   appSettings,
                   new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(_currentUserFileNames).ConfigureAwait(false),
                       new { },
                       "CurrentUser"
                   ),
                   httpClientFactory,
                   httpContextAccessor,
                   cancellationToken
               ).ConfigureAwait(false))
               ?.Data
               ?.CurrentUser
               ?? null;
    }

    /// <summary>
    /// Request user info from Metabase.
    /// </summary>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="UserInfoDto"/> if successful. </returns>
    public static async Task<UserInfoDto?> RequestUserInfo(
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var uri = new Uri(new Uri(appSettings.MetabaseHost), "/connect/userinfo");

        return await apiRequestService.Metabase().QueryRest<UserInfoDto>(
            uri,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        ).ConfigureAwait(false);
    }
}