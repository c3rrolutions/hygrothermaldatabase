using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

/// <summary>
/// Class to request *Data from Database API.
/// </summary>
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

    public static readonly string[] DataXFileNames =
    [
        "DataFields.graphql",
        "DataX.graphql"
    ];

    /// <summary>
    /// Create query to request data and get response.
    /// </summary>
    /// <exception cref="JsonException">
    /// Throws exception, when response could not be serialized.
    /// </exception>
    /// <returns> Query, variables, and response for data. </returns>
    public static async Task<(string Query, JsonElement Variables, string Response)> QueryAllMetaData<TGraphQlData>(
        Guid dataId,
        string[] filenames,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TGraphQlData : class
    {
        var variables = new { id = dataId };
        var query = await apiRequestService.ConstructGraphQlQuery(filenames);
        var response = await apiRequestService.Database().QueryGraphQl(
            appSettings,
            new GraphQLRequest(query, variables),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
        return (query, JsonSerializer.SerializeToElement(variables), response);
    }

    /// <summary>
    /// Create query to request data and get response.
    /// </summary>
    /// <param name="dataId">              Id of data to request. </param>
    /// <param name="filenames">           File names of query files. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="ApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <exception cref="Exception">
    /// Throws exception, when query could not be constructed or no response.
    /// </exception>
    /// <returns> Query and response for data. </returns>
    public static async Task<GraphQLResponse<TGraphQlData>> GetDataFromDatabase<TGraphQlData>(
        Uri databaseUri,
        Guid dataId,
        string[] filenames,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)

        where TGraphQlData : class
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(filenames),
            new { id = dataId }
        );
        return await apiRequestService.Database().QueryGraphQlFromUrl<TGraphQlData>(
            appSettings,
            databaseUri,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }
}