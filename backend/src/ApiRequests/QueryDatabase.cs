using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Database.Services;

namespace Database.ApiRequests;

public sealed class QueryDatabase
{
    private const string QueryFileName = "Database.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record Database(
         Guid Uuid,
         string Name,
         string Description,
         Uri Locator,
         DatabaseVerificationState VerificationState,
         string VerificationCode,
         DatabaseOperatorEdge Operator,
         bool IsAuthorizedToUpdateNode,
         bool IsAuthorizedToVerifyNode
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
        CancellationToken cancellationToken
    )
    {
        return (await apiRequestService.QueryGraphQl<DatabaseData>(
            GetGraphQlEndpoint(appSettings),
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(QueryFileName),
                new
                {
                    id = databaseId
                },
                "Database"
            ),
            cancellationToken
        )).Data.Database;
    }
}