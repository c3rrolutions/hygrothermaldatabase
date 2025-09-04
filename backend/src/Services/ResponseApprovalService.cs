using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.ApiRequests.Dto;
using Database.Data;
using Database.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Database.Services;

public sealed class ResponseApprovalService(
    AppSettings appSettings,
    SigningService signingService,
    ApiRequestService apiRequestService,
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<ResponseApprovalService> logger
)
{
    private sealed record CalorimetricDataResponse(CalorimetricDataDto CalorimetricData);
    private sealed record GeometricDataResponse(GeometricDataDto GeometricData);
    private sealed record HygrothermalDataResponse(HygrothermalDataDto HygrothermalData);
    private sealed record PhotovoltaicDataResponse(PhotovoltaicDataDto PhotovoltaicData);
    private sealed record OpticalDataResponse(OpticalDataDto OpticalData);

    /// <summary>
    /// Create response approval by calling graphql Api and signing responce.
    /// </summary>
    /// <param name="dataObject">        <see cref="IData"/> </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <exception cref="Exception"> Thows exception, when singing of data failed. </exception>
    /// <returns> <see cref="ResponseApproval"/> </returns>
    public async Task<ResponseApproval> CreateResponseApproval(IData dataObject, CancellationToken cancellationToken)
    {
        // Get dataset
        logger.QueryAllMetaData(dataObject.GetType(), dataObject.Id);
        var (query, variables, response) = await QueryAllMetaData(dataObject, cancellationToken);
        logger.QueryAndVariablesAndResponce(query, variables, response);
        var signature = await signingService.SignData(response);
        return new ResponseApproval(DateTime.UtcNow, signature, await signingService.ExtractFingerprint(), query, variables, response);
    }

    private async Task<(string Query, JsonElement Variables, string Response)> QueryAllMetaData(IData dataObject, CancellationToken cancellationToken)
    {
        return dataObject switch
        {
            GeometricData data => await DataApi.QueryAllMetaData<GeometricDataResponse>(data.Id, DataApi.GeometricDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            CalorimetricData data => await DataApi.QueryAllMetaData<CalorimetricDataResponse>(data.Id, DataApi.CalorimetricDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            HygrothermalData data => await DataApi.QueryAllMetaData<HygrothermalDataResponse>(data.Id, DataApi.HygrothermalDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            PhotovoltaicData data => await DataApi.QueryAllMetaData<PhotovoltaicDataResponse>(data.Id, DataApi.PhotovoltaicDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            OpticalData data => await DataApi.QueryAllMetaData<OpticalDataResponse>(data.Id, DataApi.OpticalDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            _ => throw new NotImplementedException($"Unsupported data type {typeof(IData)}"),
        };
    }
}