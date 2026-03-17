using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Logging;
using Database.Services;
using GraphQL;
using Microsoft.Extensions.Logging;

namespace Database.ApiRequests;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Response contains errors.")
    ]
    internal static partial void ResponseErrors(
        this ILogger<QueryDatabase> logger,
        [TagProvider(typeof(GraphQlErrorsTagProvider), nameof(GraphQlErrorsTagProvider.RecordTags))] GraphQLError[] errors
    );
}

public sealed class QueryDatabase(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    ILogger<QueryDatabase> logger
)
{
    private const string QueryFileName = "Database.graphql";

    public Uri GetGraphQlEndpoint =>
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

    public async Task<Database?> Do(
        Guid databaseId,
        CancellationToken cancellationToken
    )
    {
        var response = (await apiRequestService.QueryGraphQl<DatabaseData>(
            GetGraphQlEndpoint,
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(QueryFileName),
                new
                {
                    id = databaseId
                },
                "Database"
            ),
            cancellationToken
        ));
        if (response.Errors is not null)
        {
            logger.ResponseErrors(response.Errors);
        }
        return response.Data.Database; ;
    }
}