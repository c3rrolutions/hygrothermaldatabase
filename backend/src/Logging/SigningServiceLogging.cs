using Database.Services;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static partial class SigningServiceLogging
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG Fingerprint: {Fingerprint}")]
    public static partial void ExtractedFingerprint(this ILogger<SigningService> logger, string fingerprint);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG Signature: {Signature}")]
    public static partial void CreatedSignature(this ILogger<SigningService> logger, string signature);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command: {Command}")]
    public static partial void ExecuteCommand(this ILogger<SigningService> logger, string command);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GNUPG Execute Command: {Error}")]
    public static partial void ExecuteCommandDiagnostics(this ILogger<SigningService> logger, string error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command Output: {Output}")]
    public static partial void ExecuteCommandOutput(this ILogger<SigningService> logger, string output);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GNUPG: Execute Command Result: {result}")]
    public static partial void ExecuteCommandExitCode(this ILogger<SigningService> logger, int result);
}