using System;
using Database.Services;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static partial class AccessRightServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Restricted Item Id: {Id} Reason:  {Reason}")]
    public static partial void DataRestriction(this ILogger<IAccessRightsService> logger, Guid Id, string reason);
}