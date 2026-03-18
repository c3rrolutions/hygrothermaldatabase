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
        this ILogger<QueryData> logger,
        [TagProvider(typeof(GraphQlErrorsTagProvider), nameof(GraphQlErrorsTagProvider.RecordTags))] GraphQLError[] errors
    );
}

public sealed class QueryData(
    ApiRequestService apiRequestService,
    ILogger<QueryData> logger
)
{
    public async Task<GraphQLResponse<TGraphQlData>> Do<TGraphQlData>(
        Uri databaseUrl,
        Guid dataId,
        string query,
        CancellationToken cancellationToken)

        where TGraphQlData : class
    {
        var response = await apiRequestService.QueryGraphQl<TGraphQlData>(
            databaseUrl,
            new GraphQLRequest(
                query,
                new { id = dataId }
            ),
            cancellationToken
        );
        if (response.Errors is not null)
        {
            logger.ResponseErrors(response.Errors);
        }
        return response;
    }
}