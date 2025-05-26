using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.ApiRequests.Dto;
using Database.Data;
using Database.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Database.Services;

/// <summary>
/// Implementation of <see cref="IResponseApprovalService"/>
/// </summary>
/// <param name="appSettings">         <see cref="AppSettings"/> </param>
/// <param name="signingService">      <see cref="ISigningService"/> </param>
/// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
/// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
/// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
/// <param name="logger">              <see cref="ILogger"/> </param>
public class ResponseApprovalService(
    AppSettings appSettings,
    ISigningService signingService,
    IApiRequestService apiRequestService,
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<IResponseApprovalService> logger) : IResponseApprovalService
{
    private sealed record CalorimetricDataResponse(CalorimetricDataDto CalorimetricData);
    private sealed record GeometricDataResponse(GeometricDataDto GeometricData);
    private sealed record HygrothermalDataResponse(HygrothermalDataDto HygrothermalData);
    private sealed record PhotovoltaicDataResponse(PhotovoltaicDataDto PhotovoltaicData);
    private sealed record OpticalDataResponse(OpticalDataDto OpticalData);

    /// <inheritdoc/>
    public async Task<ResponseApproval> CreateResponseApproval(IData dataObject, CancellationToken cancellationToken)
    {
        // Get dataset
        logger.GetQueryAndResponse(dataObject.GetType().Name);
        var queryAdnResponse = await GetQueryAndResponse(dataObject, cancellationToken).ConfigureAwait(false);

        logger.QueryAndResponce(queryAdnResponse.Query, queryAdnResponse.Response);

        var signatureResult = await signingService.SignData(queryAdnResponse.Response).ConfigureAwait(false);

        if (!signatureResult.Success)
        {
            throw new InvalidOperationException($"Signing of data failed! {signatureResult.Output}");
        }

        return new ResponseApproval(DateTime.UtcNow, signatureResult.Output, signingService.Fingerprint, queryAdnResponse.Query, queryAdnResponse.Response);
    }

    private async Task<(string Query, string Response)> GetQueryAndResponse(IData dataObject, CancellationToken cancellationToken)
    {
        switch (dataObject)
        {
            case GeometricData data:
                return await DataApi.CreateQueryAndGetResponse<GeometricDataResponse>(data.Id, DataApi.GeometricDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case CalorimetricData data:
                return await DataApi.CreateQueryAndGetResponse<CalorimetricDataResponse>(data.Id, DataApi.CalorimetricDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case HygrothermalData data:
                return await DataApi.CreateQueryAndGetResponse<HygrothermalDataResponse>(data.Id, DataApi.HygrothermalDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case PhotovoltaicData data:
                return await DataApi.CreateQueryAndGetResponse<PhotovoltaicDataResponse>(data.Id, DataApi.PhotovoltaicDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case OpticalData data:
                return await DataApi.CreateQueryAndGetResponse<OpticalDataResponse>(data.Id, DataApi.OpticalDataFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            default:
                throw new NotImplementedException("Unknown IData object.");
        }
    }
}