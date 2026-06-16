using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Database.Services;

[SuppressMessage("Naming", "CA1707")]
public enum SigantureVerificationResult
{
    FAILED_RECEIVING_KEY,
    GOOD_SIGNATURE,
    BAD_SIGNATURE
}

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GnuPG Fingerprint: {Fingerprint}")]
    public static partial void ExtractedFingerprint(this ILogger<SigningService> logger, string fingerprint);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GnuPG Signature: {Signature}")]
    public static partial void CreatedSignature(this ILogger<SigningService> logger, string signature);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to receive the key '{Fingerprint}' from the keyserver '{KeyServerUrl}' with standard output '{Output}' and diagnostic information '{Diagnostics}'")]
    public static partial void FailedToReceiveKey(this ILogger<SigningService> logger, string fingerprint, Uri keyServerUrl, string output, string diagnostics);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to verify the signature '{Signature}' for the message '{Message}' and the fingerprint '{Fingerprint}' with standard output '{Output}' and diagnostic information '{Diagnostics}'")]
    public static partial void FailedToVerifySignature(this ILogger<SigningService> logger, string fingerprint, string signature, string message, string output, string diagnostics);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GnuPG: Execute Command: {Command}")]
    public static partial void ExecuteCommand(this ILogger<SigningService> logger, string command);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "GnuPG Execute Command: {Error}")]
    public static partial void ExecuteCommandDiagnostics(this ILogger<SigningService> logger, string error);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GnuPG: Execute Command Output: {Output}")]
    public static partial void ExecuteCommandOutput(this ILogger<SigningService> logger, string output);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "GnuPG: Execute Command Result: {result}")]
    public static partial void ExecuteCommandExitCode(this ILogger<SigningService> logger, int result);
}

public sealed class SigningService(
    AppSettings appSettings,
    ILogger<SigningService> logger)
{
    public static readonly Uri KeyServerUrl = new("hkps://keys.openpgp.org/", UriKind.Absolute);

    public async Task AssertGnuPgKeyExistence()
    {
        var (success, output, diagnostics) = await ExecuteCommand(
            $"gpg --batch --with-colons --list-keys | grep '{appSettings.GnupgSecretSigningKey.Fingerprint}'"
        );
        if (!success)
        {
            throw new InvalidOperationException($"There is no GnuPG key with the fingerprint '{appSettings.GnupgSecretSigningKey.Fingerprint}'. The command gave the standard output '{output}' and the diagnostic information '{diagnostics}'.");
        }
    }

    public async Task<(string Signature, string Fingerprint)> SignData(string data)
    {
        var (success, signature, diagnostics) = await ExecuteCommand(
            $"gpg --pinentry-mode loopback --batch --passphrase '{appSettings.GnupgSecretSigningKey.Passphrase}' --detach-sig --armor --local-user '{appSettings.GnupgSecretSigningKey.Fingerprint}'",
            data
        );
        if (!success)
        {
            throw new InvalidOperationException($"Failed to create signature with standard output '{signature}' and diagnostic information '{diagnostics}'");
        }
        logger.CreatedSignature(signature);
        return (signature, appSettings.GnupgSecretSigningKey.Fingerprint);
    }

    public async Task<SigantureVerificationResult> VerifySignature(
        string message, string signature, string fingerprint
    )
    {
        if (!await ReceiveKey(fingerprint))
        {
            return SigantureVerificationResult.FAILED_RECEIVING_KEY;
        }
        var temporaryMessageFilePath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(temporaryMessageFilePath, message);
            var (success, output, diagnostics) = await ExecuteCommand(
                $"gpg --pinentry-mode loopback --batch --always-trust --assert-signer '{fingerprint}' --verify - '{temporaryMessageFilePath}'",
                signature
            );
            if (!success)
            {
                logger.FailedToVerifySignature(fingerprint, signature, message, output, diagnostics);
            }
            return success
                ? SigantureVerificationResult.GOOD_SIGNATURE
                : SigantureVerificationResult.BAD_SIGNATURE;
        }
        finally
        {
            if (File.Exists(temporaryMessageFilePath))
            {
                File.Delete(temporaryMessageFilePath);
            }
        }
    }

    private async Task<bool> ReceiveKey(string fingerprint)
    {
        var (success, output, diagnostics) = await ExecuteCommand(
            $"gpg --pinentry-mode loopback --batch --keyserver {KeyServerUrl.AbsoluteUri} --receive-keys {fingerprint}"
        );
        if (!success)
        {
            logger.FailedToReceiveKey(fingerprint, KeyServerUrl, output, diagnostics);
        }
        return success;
    }

    private async Task<(bool Success, string Output, string Diagnostics)> ExecuteCommand(
        string command,
        string? input = null
    )
    {
        logger.ExecuteCommand(command);
        var process = Process.Start(
            new ProcessStartInfo()
            {
                FileName = "bash",
                ArgumentList = {
                    "-o", "errexit",
                    "-o", "errtrace",
                    "-o", "nounset",
                    "-o", "pipefail",
                    "-c", command
                },
                ErrorDialog = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        ) ?? throw new InvalidOperationException($"The process for command '{command}' with the input '{input}' failed to start.");
        if (input is not null)
        {
            await process.StandardInput.WriteLineAsync(input);
            await process.StandardInput.FlushAsync();
        }
        process.StandardInput.Close();
        var output = process.StandardOutput.ReadToEnd();
        var diagnostics = process.StandardError.ReadToEnd();
        await process.WaitForExitAsync();
        logger.ExecuteCommandOutput(output);
        logger.ExecuteCommandDiagnostics(diagnostics);
        logger.ExecuteCommandExitCode(process.ExitCode);
        return (process.ExitCode is 0, output, diagnostics);
    }
}