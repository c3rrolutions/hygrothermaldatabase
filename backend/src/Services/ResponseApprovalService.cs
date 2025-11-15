using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Database.Data;
using GraphQL;
using Database.Json;
using HotChocolate;
using System.Linq;

namespace Database.Services;

public static partial class ResponseApprovalServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Querying all meta data for {DataType} with ID {Id}")]
    public static partial void QueryAllMetaData(this ILogger<ResponseApprovalService> logger, Type dataType, Guid id);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Query, variables, and response: {Query}, {Variables}, and {Response}")]
    public static partial void QueryAndVariablesAndResponce(this ILogger<ResponseApprovalService> logger, string query, JsonElement variables, string response);
}

public sealed class ResponseApprovalService(
    AppSettings appSettings,
    SigningService signingService,
    ApiRequestService apiRequestService,
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
        var typedResponse = JsonSerializer.Deserialize<GraphQLResponse<JsonElement>>(response, JsonSerializerSettings.GraphQl)
             ?? throw new JsonException($"Failed to deserialize the GraphQL response: {response}");
        if (typedResponse.Errors is not null && typedResponse.Errors.Length >= 1)
        {
            throw new GraphQLException($"The GraphQL response contains the following errors: {string.Join(", ", typedResponse.Errors.Select(_ => $"'{_.Message}' [{string.Join(", ", _.Extensions?.Select(e => $"{e.Key}: '{e.Value}'") ?? [])}] ({string.Join(".", _.Path?.Select(p => p.ToString()) ?? [])})"))}");
        }
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
            CalorimetricData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.CalorimetricDataFileNames, appSettings, apiRequestService, cancellationToken),
            GeometricData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.GeometricDataFileNames, appSettings, apiRequestService, cancellationToken),
            HygrothermalData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.HygrothermalDataFileNames, appSettings, apiRequestService, cancellationToken),
            OpticalData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.OpticalDataFileNames, appSettings, apiRequestService, cancellationToken),
            PhotovoltaicData data => await ApiRequests.QueryAllMetaData.Do(data.Id, ApiRequests.QueryAllMetaData.PhotovoltaicDataFileNames, appSettings, apiRequestService, cancellationToken),
            _ => throw new ArgumentOutOfRangeException($"Unsupported data type {typeof(T)}"),
        };
    }
}