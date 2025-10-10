using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;

namespace Database.ApiRequests;

public sealed class QueryData
{
    public static async Task<GraphQLResponse<TGraphQlData>> Do<TGraphQlData>(
        Uri databaseUrl,
        Guid dataId,
        string query,
        ApiRequestService apiRequestService,
        CancellationToken cancellationToken)

        where TGraphQlData : class
    {
        return await apiRequestService.QueryGraphQl<TGraphQlData>(
            databaseUrl,
            new GraphQLRequest(
                query,
                new { id = dataId }
            ),
            cancellationToken
        );
    }
}