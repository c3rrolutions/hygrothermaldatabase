using Database.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static partial class SigningServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG Fingerprint: {Fingerprint}")]
    public static partial void Fingerprint(this ILogger<ISigningService> logger, string fingerprint);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Failed to get fingerprint.")]
    public static partial void FingerprintError(this ILogger<ISigningService> logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG Signature: {Signature}")]
    public static partial void Signature(this ILogger<ISigningService> logger, string signature);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Failed to create signature: {Output}")]
    public static partial void SignatureError(this ILogger<ISigningService> logger, string output);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command: {Command}")]
    public static partial void ExecuteCommand(this ILogger<ISigningService> logger, string command);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG Execute Command: {Error}")]
    public static partial void ExecuteCommandError(this ILogger<ISigningService> logger, string error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command Output: {Output}")]
    public static partial void ExecuteCommandOutput(this ILogger<ISigningService> logger, string output);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command Result: {result}")]
    public static partial void ExecuteCommandResult(this ILogger<ISigningService> logger, int result);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Write file exception: {Exception}")]
    public static partial void WriteFileException(this ILogger<ISigningService> logger, string exception);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG: Read file exception: {Exception}")]
    public static partial void ReadFileException(this ILogger<ISigningService> logger, string exception);
}