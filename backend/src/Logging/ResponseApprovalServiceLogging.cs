using Database.Services;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static partial class ResponseApprovalServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Getting query and response for {DataType}")]
    public static partial void GetQueryAndResponse(this ILogger<ResponseApprovalService> logger, string dataType);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Query and response: {Query} \n {Response}")]
    public static partial void QueryAndResponce(this ILogger<ResponseApprovalService> logger, string query, string response);
}