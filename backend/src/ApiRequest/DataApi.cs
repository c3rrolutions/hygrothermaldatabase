using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest.Dto;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequest;

/// <summary>
/// Class to request XData from Database API.
/// </summary>
public class DataApi
{
    private static readonly string[] _calorimetricDataFileNames =
    {
        "CalorimetricData.graphql"
    };

    private static readonly string[] _geometricDataFileNames =
    {
        "GeometricData.graphql"
    };

    private static readonly string[] _hygrothermalDataFileNames =
    {
        "HygrothermalData.graphql"
    };

    private static readonly string[] _photovoltaicDataFileNames =
    {
        "PhotovoltaicData.graphql"
    };

    private static readonly string[] _opticalDataFileNames =
    {
        "OpticalData.graphql"
    };

    private sealed record CalorimetricDataResponse(CalorimetricDataDto CalorimetricData);
    private sealed record GeometricDataResponse(GeometricDataDto GeometricData);
    private sealed record HygrothermalDataResponse(HygrothermalDataDto HygrothermalData);
    private sealed record PhotovoltaicDataResponse(PhotovoltaicDataDto PhotovoltaicData);
    private sealed record OpticalDataResponse(OpticalDataDto OpticalData);

    /// <summary>
    /// Create query to request calorimetric data and get response.
    /// </summary>
    /// <param name="dataId">              Id of data to request. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> Query and response for calorimetric data. </returns>
    public static async Task<(string? Query, string? Response)> CreateQueryAndGetResponseCalorimetricData(
        Guid dataId,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(_calorimetricDataFileNames).ConfigureAwait(false),
            dataId,
            "GetCalorimetricData"
            );
        var response = await apiRequestService.Database().QueryGraphQl<CalorimetricDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, JsonSerializer.Serialize(response));
    }

    /// <summary>
    /// Create query to request geometric data and get response.
    /// </summary>
    /// <param name="dataId">              Id of data to request. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> Query and response for geometric data. </returns>
    public static async Task<(string? Query, string? Response)> CreateQueryAndGetResponseGeometricData(
        Guid dataId,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(_geometricDataFileNames).ConfigureAwait(false),
            new { uuid = dataId },
            "GeometricData"
            );
        var response = await apiRequestService.Database().QueryGraphQl<GeometricDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (Query: request.Query, JsonSerializer.Serialize(response));
    }

    /// <summary>
    /// Create query to request hygrothemal data and get response.
    /// </summary>
    /// <param name="dataId">              Id of data to request. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> Query and response for hygrothermal data. </returns>
    public static async Task<(string? Query, string? Response)> CreateQueryAndGetResponseHygrothermalData(
        Guid dataId,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(_hygrothermalDataFileNames).ConfigureAwait(false),
            dataId,
            "GetHygrothermalData"
            );
        var response = await apiRequestService.Database().QueryGraphQl<HygrothermalDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, JsonSerializer.Serialize(response));
    }

    /// <summary>
    /// Create query to request photovoltaic data and get response.
    /// </summary>
    /// <param name="dataId">              Id of data to request. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> Query and response for photovoltaic data. </returns>
    public static async Task<(string? Query, string? Response)> CreateQueryAndGetResponsePhotovoltaicData(
        Guid dataId,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(_photovoltaicDataFileNames).ConfigureAwait(false),
            dataId,
            "GetPhotovoltaicData"
            );
        var response = await apiRequestService.Database().QueryGraphQl<PhotovoltaicDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, JsonSerializer.Serialize(response));
    }

    /// <summary>
    /// Create query to request optical data and get response.
    /// </summary>
    /// <param name="dataId">              Id of data to request. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> Query and response for optical data. </returns>
    public static async Task<(string? Query, string? Response)> CreateQueryAndGetResponseOpticalData(
        Guid dataId,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        var request = new GraphQLRequest(
            await apiRequestService.ConstructGraphQlQuery(_opticalDataFileNames).ConfigureAwait(false),
            dataId,
            "GetOpticalData"
            );
        var response = await apiRequestService.Database().QueryGraphQl<OpticalDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, JsonSerializer.Serialize(response));
    }
}