using System;
using System.Text.Json;
using Database.Services;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

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