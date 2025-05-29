using Database.Services;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static partial class SigningServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG Fingerprint: {Fingerprint}")]
    public static partial void Fingerprint(this ILogger<SigningService> logger, string fingerprint);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Failed to get fingerprint.")]
    public static partial void FingerprintError(this ILogger<SigningService> logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG Signature: {Signature}")]
    public static partial void Signature(this ILogger<SigningService> logger, string signature);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Failed to create signature: {Output}")]
    public static partial void SignatureError(this ILogger<SigningService> logger, string output);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command: {Command}")]
    public static partial void ExecuteCommand(this ILogger<SigningService> logger, string command);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG Execute Command: {Error}")]
    public static partial void ExecuteCommandError(this ILogger<SigningService> logger, string error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command Output: {Output}")]
    public static partial void ExecuteCommandOutput(this ILogger<SigningService> logger, string output);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command Result: {result}")]
    public static partial void ExecuteCommandResult(this ILogger<SigningService> logger, int result);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Write file exception: {Exception}")]
    public static partial void WriteFileException(this ILogger<SigningService> logger, string exception);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Read file exception: {Exception}")]
    public static partial void ReadFileException(this ILogger<SigningService> logger, string exception);
}