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

public class ResponseApprovalService(
    AppSettings appSettings,
    ISigningService signingService,
    IApiRequestService apiRequestService,
    IHttpClientFactory httpClientFactory,
    ILogger<IResponseApprovalService> logger) : IResponseApprovalService
{
    public async Task<ResponseApproval> CreateResponseApproval(object dataObject, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    {
        // Get dataset
        logger.GetQueryAndResponse(dataObject.GetType().Name);
        var queryAdnResponce = await GetQueryAndResponse(dataObject, httpContextAccessor, cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrEmpty(queryAdnResponce.Query))
        {
            throw new ArgumentNullException(queryAdnResponce.Query);
        }
        if (string.IsNullOrEmpty(queryAdnResponce.Response))
        {
            throw new ArgumentNullException(queryAdnResponce.Response);
        }

        logger.QueryAndResponce(queryAdnResponce.Query, queryAdnResponce.Response);

        var signatureResult = await signingService.SignData(queryAdnResponce.Response).ConfigureAwait(false);

        if (!signatureResult.Success)
        {
            throw new ArgumentException("Signing failed");
        }

        return new ResponseApproval(DateTime.Now, signatureResult.Signature, signingService.GetFingerprint(), queryAdnResponce.Query, queryAdnResponce.Response);
    }

    private async Task<(string? Query, string? Response)> GetQueryAndResponse(object dataObject, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    {
        switch (dataObject)
        {
            case GeometricData data:
                return await DataApi.GetQueryAndResponceGeometricData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case CalorimetricData data:
                return await DataApi.GetQueryAndResponceCalorimetricData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case HygrothermalData data:
                return await DataApi.GetQueryAndResponceHygrothermalData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case PhotovoltaicData data:
                return await DataApi.GetQueryAndResponcePhotovoltaicData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            case OpticalData data:
                return await DataApi.GetQueryAndResponceOpticalData(data.Id, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

            default:
                return (null, null);
        }
    }
}