using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest.Dto;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequest;

public class UserApi
{
    private static readonly string[] _currentUserFileNames =
    {
        "CurrentUser.graphql"
    };

    private sealed record CurrentUserData(CurrentUserDto CurrentUser);

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