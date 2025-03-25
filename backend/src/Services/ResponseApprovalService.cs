using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest;
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
    /// <inheritdoc/>
    public async Task<ResponseApproval> CreateResponseApproval(IData dataObject, CancellationToken cancellationToken)
    {
        // Get dataset
        logger.GetQueryAndResponse(dataObject.GetType().Name);
        var queryAdnResponse = await GetQueryAndResponse(dataObject, cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrEmpty(queryAdnResponse.Query))
        {
            throw new ArgumentNullException(queryAdnResponse.Query);
        }
        if (string.IsNullOrEmpty(queryAdnResponse.Response))
        {
            throw new ArgumentNullException(queryAdnResponse.Response);
        }

        logger.QueryAndResponce(queryAdnResponse.Query, queryAdnResponse.Response);

        var signatureResult = await signingService.SignData(queryAdnResponse.Response).ConfigureAwait(false);

        if (!signatureResult.Success)
        {
            throw new ArgumentException("Signing failed");
        }

        return new ResponseApproval(DateTime.UtcNow, signatureResult.Signature, signingService.Fingerprint, queryAdnResponse.Query, queryAdnResponse.Response);
    }

    private async Task<(string? Query, string? Response)> GetQueryAndResponse(IData dataObject, CancellationToken cancellationToken)
    {
        switch (dataObject)
        {
            case GeometricData data:
                return await DataApi.CreateQueryAndGetResponseGeometricData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case CalorimetricData data:
                return await DataApi.CreateQueryAndGetResponseCalorimetricData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case HygrothermalData data:
                return await DataApi.CreateQueryAndGetResponseHygrothermalData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case PhotovoltaicData data:
                return await DataApi.CreateQueryAndGetResponsePhotovoltaicData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case OpticalData data:
                return await DataApi.CreateQueryAndGetResponseOpticalData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            default:
                return (null, null);
        }
    }
}