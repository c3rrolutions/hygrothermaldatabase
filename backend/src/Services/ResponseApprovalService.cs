using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
    /// <summary>
    /// Create response approval by calling graphql Api and signing responce.
    /// </summary>
    /// <param name="dataObject">        <see cref="IData"/> </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <exception cref="Exception"> Thows exception, when singing of data failed. </exception>
    /// <returns> <see cref="ResponseApproval"/> </returns>
    public async Task<ResponseApproval> CreateResponseApproval<T>(T dataObject, CancellationToken cancellationToken)
        where T : IData
    {
        // Get dataset
        logger.QueryAllMetaData(dataObject.GetType(), dataObject.Id);
        var (query, variables, response) = await QueryAllMetaData(dataObject, cancellationToken);
        logger.QueryAndVariablesAndResponce(query, variables, response);
        var (signature, fingerprint) = await signingService.SignData(response);
        return new ResponseApproval(
            DateTime.UtcNow,
            signature,
            fingerprint,
            query,
            variables,
            response,
            appSettings.OperatorId
        );
    }

    private async Task<(string Query, JsonElement Variables, string Response)> QueryAllMetaData<T>(T dataObject, CancellationToken cancellationToken)
        where T : IData
    {
        return dataObject switch
        {
            GeometricData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.GeometricDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            CalorimetricData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.CalorimetricDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            HygrothermalData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.HygrothermalDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            PhotovoltaicData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.PhotovoltaicDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            OpticalData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.OpticalDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken),
            _ => throw new NotImplementedException($"Unsupported data type {typeof(T)}"),
        };
    }
}