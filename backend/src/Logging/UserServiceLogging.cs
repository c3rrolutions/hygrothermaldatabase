using Database.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static partial class UserServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Extracted Bearer Token: {Token}")]
    public static partial void ExtractedToken(this ILogger<IUserService> logger, string? token);
}