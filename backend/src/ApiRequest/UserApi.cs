using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest.Dto;
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
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        return (await ApiRequestService.QueryGraphQl<CurrentUserData>(
                   appSettings,
                   new GraphQLRequest(
                       await ApiRequestService.ConstructGraphQlQuery(
                           _currentUserFileNames
                       ).ConfigureAwait(false),
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
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var uri = new Uri(new Uri(appSettings.MetabaseHost), "/connect/userinfo");

        return await ApiRequestService.QueryRest<UserInfoDto>(
            uri,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }
}