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
using HotChocolate.Execution;
using Database.ApiRequests;
using System.Collections.Generic;

namespace Database.Services;

public static partial class ResponseApprovalServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Querying all meta data for {DataType} with ID {Id}")]
    public static partial void Query(this ILogger<ResponseApprovalService> logger, Type dataType, Guid id);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Query, variables, and response: {Query}, {Variables}, and {Response}")]
    public static partial void QueryAndVariablesAndResponse(this ILogger<ResponseApprovalService> logger, string query, JsonElement variables, string response);
}

public sealed class ResponseApprovalService(
    AppSettings appSettings,
    SigningService signingService,
    IRequestExecutorResolver requestExecutorResolver,
    ILogger<ResponseApprovalService> logger
)
{
    private static readonly string[] s_calorimetricDataQueryFileNames =
    [
        "DataFields.graphql",
        "CalorimetricDataFields.graphql",
        "CalorimetricData.graphql"
    ];

    private static readonly string[] s_geometricDataQueryFileNames =
    [
        "DataFields.graphql",
        "GeometricDataFields.graphql",
        "GeometricData.graphql"
    ];

    private static readonly string[] s_hygrothermalDataQueryFileNames =
    [
        "DataFields.graphql",
        "HygrothermalDataFields.graphql",
        "HygrothermalData.graphql"
    ];

    private static readonly string[] s_photovoltaicDataQueryFileNames =
    [
        "DataFields.graphql",
        "PhotovoltaicDataFields.graphql",
        "PhotovoltaicData.graphql"
    ];

    private static readonly string[] s_opticalDataQueryFileNames =
    [
        "DataFields.graphql",
        "OpticalDataFields.graphql",
        "OpticalData.graphql"
    ];

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
        var (query, variables, response) = await Query(dataObject, cancellationToken);
        logger.QueryAndVariablesAndResponse(query, variables, response);
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

    private async Task<(string Query, JsonElement Variables, string Response)> Query<T>(T dataObject, CancellationToken cancellationToken)
        where T : IData
    {
        logger.Query(dataObject.GetType(), dataObject.Id);
        return dataObject switch
        {
            CalorimetricData data => await Query(data.Id, s_calorimetricDataQueryFileNames, cancellationToken),
            GeometricData data => await Query(data.Id, s_geometricDataQueryFileNames, cancellationToken),
            HygrothermalData data => await Query(data.Id, s_hygrothermalDataQueryFileNames, cancellationToken),
            OpticalData data => await Query(data.Id, s_opticalDataQueryFileNames, cancellationToken),
            PhotovoltaicData data => await Query(data.Id, s_photovoltaicDataQueryFileNames, cancellationToken),
            _ => throw new ArgumentOutOfRangeException($"Unsupported data type {typeof(T)}"),
        };
    }

    private async Task<(string Query, JsonElement Variables, string Response)> Query(
        Guid dataId,
        string[] queryFileNames,
        CancellationToken cancellationToken
    )
    {
        var query = await GraphQlQueryHelpers.Construct(queryFileNames);
        var variables = new Dictionary<string, object?> { ["id"] = dataId };
        var operationRequest = OperationRequestBuilder.New()
            .SetDocument(query)
            .SetVariableValues(variables)
            .Build();
        var requestExecutor = await requestExecutorResolver.GetRequestExecutorAsync(cancellationToken: cancellationToken);
        var executionResult = await requestExecutor.ExecuteAsync(operationRequest, cancellationToken);
        var response = executionResult.ToJson(withIndentations: false);
        return (query, JsonSerializer.SerializeToElement(variables), response);
    }
}