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
    private const string QueryFileName = "Database.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        ApiRequestService.MetabaseGraphQlEndpoint(appSettings);

    public sealed record Database(
         Guid Uuid,
         string Name,
         string Description,
         Uri Locator,
         DatabaseVerificationState VerificationState,
         string VerificationCode,
         DatabaseOperatorEdge Operator,
         bool CanCurrentUserUpdateNode,
         bool CanCurrentUserVerifyNode
    );

    public enum DatabaseVerificationState
    {
        PENDING,
        VERIFIED
    }

    public sealed record DatabaseOperatorEdge(
        Institution Node
    );

    public sealed record Institution(
         Guid Uuid
    );

    private sealed record DatabaseData(Database? Database);

    public static async Task<Database?> Do(
        Guid databaseId,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        return (await apiRequestService.Metabase().QueryGraphQl<DatabaseData>(
            appSettings,
            new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(QueryFileName),
                new
                {
                    id = databaseId
                },
                "Database"
            ),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        )).Data.Database;
    }
}