using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

/// <summary>
/// Class to request XData from Database API.
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
    public static async Task<(string Query, string Response)> CreateQueryAndGetResponse<TGraphQlResponse>(
        Guid dataId,
        string[] filenames,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)

        where TGraphQlResponse : class
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(filenames).ConfigureAwait(false),
            new { uuid = dataId },
            ""
        );
        var query = request.Query ?? throw new InvalidOperationException("Failed to construct GraphQL query.");

        var responseObject = await apiRequestService.Database().QueryGraphQl<TGraphQlResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        ).ConfigureAwait(false);
        var response = JsonSerializer.Serialize(responseObject);
        if (string.IsNullOrEmpty(response))
        {
            throw new InvalidOperationException("No response for query.");
        }

        return (query, response);
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
    public static async Task<GraphQLResponse<TGraphQlResponse>> GetDataFromDatabase<TGraphQlResponse>(
        Uri databaseUri,
        Guid dataId,
        string[] filenames,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)

        where TGraphQlResponse : class
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(filenames).ConfigureAwait(false),
            new { uuid = dataId },
            ""
        );
        return await apiRequestService.Database().QueryGraphQlFromUrl<TGraphQlResponse>(
            appSettings,
            databaseUri,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }
}