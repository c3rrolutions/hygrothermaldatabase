using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

public sealed class QueryDatabase
{
    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        ApiRequestService.MetabaseGraphQlEndpoint(appSettings);

    private sealed record DatabaseData(Models.Database Database);

    private static readonly string[] s_queryDatabaseFileNames =
    [
        "Database.graphql"
    ];

    public static async Task<Models.Database?> Do(
        Guid databaseId,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var response = await apiRequestService.Metabase().QueryGraphQl<DatabaseData>(
                    appSettings,
                    new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(s_queryDatabaseFileNames),
                        new
                        {
                            id = databaseId
                        },
                        "Database"
                    ),
                    httpClientFactory,
                    httpContextAccessor,
                    cancellationToken
                );
        return response.Data?.Database;
    }
}