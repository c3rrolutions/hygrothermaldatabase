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
public sealed class UserApi
{
    private static readonly string[] s_currentUserFileNames =
    [
        "CurrentUser.graphql"
    ];

    private sealed record CurrentUserData(CurrentUserDto CurrentUser);

    /// <summary>
    /// Request current user from Metabase.
    /// </summary>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="ApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="CurrentUserDto"/> if successful. </returns>
    public static async Task<CurrentUserDto?> RequestCurrentUser(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        return (await apiRequestService.Metabase().QueryGraphQl<CurrentUserData>(
                   appSettings,
                   new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(s_currentUserFileNames),
                       new {
                        databaseId = appSettings.DatabaseId
                       },
                       "CurrentUser"
                   ),
                   httpClientFactory,
                   httpContextAccessor,
                   cancellationToken
               ))
               ?.Data
               ?.CurrentUser
               ?? null;
    }

    /// <summary>
    /// Request user info from Metabase.
    /// </summary>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="ApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="UserInfoDto"/> if successful. </returns>
    public static Task<UserInfoDto> RequestUserInfo(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        return apiRequestService.Metabase().PerformHttpGetRequest<UserInfoDto>(
            new Uri(new Uri(appSettings.MetabaseHost, UriKind.Absolute), "/connect/userinfo"),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }
}