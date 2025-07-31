using System;
using Database.Services;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static partial class AccessRightServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Cannot apply access rights because the user or application is unknown. The fetched user ID is {UserId} and the application ID is {ApplicationId}.")]
    public static partial void UnknownUserOrApplication(this ILogger<AccessRightsService> logger, Guid? userId, string? applicationId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Restricted Item Id: {Id} Reason:  {Reason}")]
    public static partial void DataRestriction(this ILogger<AccessRightsService> logger, Guid Id, string reason);
}