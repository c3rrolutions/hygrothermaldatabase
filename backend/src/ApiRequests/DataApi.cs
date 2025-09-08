using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

public sealed class DataApi
{
    public static readonly string[] CalorimetricDataFileNames =
    [
        "DataFields.graphql",
        "CalorimetricDataFields.graphql",
        "CalorimetricData.graphql"
    ];

    public static readonly string[] GeometricDataFileNames =
    [
        "DataFields.graphql",
        "GeometricDataFields.graphql",
        "GeometricData.graphql"
    ];

    public static readonly string[] HygrothermalDataFileNames =
    [
        "DataFields.graphql",
        "HygrothermalDataFields.graphql",
        "HygrothermalData.graphql"
    ];

    public static readonly string[] PhotovoltaicDataFileNames =
    [
        "DataFields.graphql",
        "PhotovoltaicDataFields.graphql",
        "PhotovoltaicData.graphql"
    ];

    public static readonly string[] OpticalDataFileNames =
    [
        "DataFields.graphql",
        "OpticalDataFields.graphql",
        "OpticalData.graphql"
    ];

    public static async Task<(string Query, JsonElement Variables, string Response)> QueryAllMetaData(
        Guid dataId,
        string[] fileNames,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        var variables = new { id = dataId };
        var query = await apiRequestService.ConstructGraphQlQuery(fileNames);
        var response = await apiRequestService.Database().QueryGraphQl(
            appSettings,
            new GraphQLRequest(query, variables),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
        return (query, JsonSerializer.SerializeToElement(variables), response);
    }

    public static async Task<GraphQLResponse<TGraphQlData>> QueryDataFromDatabase<TGraphQlData>(
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