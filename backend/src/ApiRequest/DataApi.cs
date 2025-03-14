using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequest;

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

    private sealed record CalorimetricDataResponse(CalorimetricData CalorimetricData);
    private sealed record GeometricDataResponse(GeometricData GeometricData);
    private sealed record HygrothermalDataResponse(HygrothermalData HygrothermalData);
    private sealed record PhotovoltaicDataResponse(PhotovoltaicData PhotovoltaicData);
    private sealed record OpticalDataResponse(OpticalData OpticalData);

    public static async Task<(string? Query, string? Response)> GetQueryAndResponceCalorimetricData(
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
            new { uuid = dataId },
            "Data"
            );
        var response = await apiRequestService.Database().QueryGraphQl<CalorimetricDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, response.ToString());
    }

    public static async Task<(string? Query, string? Response)> GetQueryAndResponceGeometricData(
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
            "Data"
            );
        var response = await apiRequestService.Database().QueryGraphQl<GeometricDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (Query: request.Query, response.ToString());
    }

    public static async Task<(string? Query, string? Response)> GetQueryAndResponceHygrothermalData(
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
            new { uuid = dataId },
            "Data"
            );
        var response = await apiRequestService.Database().QueryGraphQl<HygrothermalDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, response.ToString());
    }

    public static async Task<(string? Query, string? Response)> GetQueryAndResponcePhotovoltaicData(
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
            new { uuid = dataId },
            "Data"
            );
        var response = await apiRequestService.Database().QueryGraphQl<PhotovoltaicDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, response.ToString());
    }

    public static async Task<(string? Query, string? Response)> GetQueryAndResponceOpticalData(
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
            new { uuid = dataId },
            "Data"
            );
        var response = await apiRequestService.Database().QueryGraphQl<OpticalDataResponse>(
            appSettings,
            request,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false);

        return (request.Query, response.ToString());
    }
}