using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.Metabase;

public class QueryingCurrentUser
{
    private static readonly string[] _currentUserFileNames =
    {
        "CurrentUser.graphql"
    };

    private sealed record CurrentUserResponse(CurrentUser CurrentUser);

    public static async Task<CurrentUser?> Query(
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        return (await QueryingMetabase.QueryGraphQl<CurrentUserResponse>(
                   appSettings,
                   new GraphQLRequest(
                       await QueryingMetabase.ConstructGraphQlQuery(
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
}