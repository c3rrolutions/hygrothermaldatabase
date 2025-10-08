using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

public sealed class QueryData
{
    public static async Task<GraphQLResponse<TGraphQlData>> Do<TGraphQlData>(
        Uri databaseUrl,
        Guid dataId,
        string query,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)

        where TGraphQlData : class
    {
        return await apiRequestService.Database().QueryGraphQl<TGraphQlData>(
            appSettings,
            databaseUrl,
            new GraphQLRequest(
                query,
                new { id = dataId }
            ),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }
}