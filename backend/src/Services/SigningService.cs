using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Database.Logging;
using Microsoft.Extensions.Logging;
using Quartz.Util;

namespace Database.Services;

public sealed class SigningService(
    AppSettings appSettings,
    ILogger<SigningService> logger)
{
    public async Task Initialize()
    {
        var (success, output, diagnostics) = await ExecuteCommand(
            $"gpg --batch --passphrase '{appSettings.GnupgPrivateKeyPassphrase}' --import './gpg-keys/{appSettings.GnupgPrivateKeyFilename}'"
        );
    }

    public async Task<string> SignData(string data)
    {
        var (success, signature, diagnostics) = await ExecuteCommand(
            $"gpg --pinentry-mode loopback --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --detach-sig --armor --local-user {await ExtractFingerprint()}",
            data
        );
        if (!success)
        {
            throw new InvalidOperationException($"Failed to create signature with standard output {signature} and diagnostic information {diagnostics}");
        }
        logger.CreatedSignature(signature);
        return signature;
    }

    public async Task<string> ExtractFingerprint()
    {
        var (success, fingerprint, diagnostics) = await ExecuteCommand("gpg --list-secret-keys --with-colons --keyid-format=long | awk -F: '$1==\"fpr\" {printf $10; exit}'");
        if (!success || fingerprint.IsNullOrWhiteSpace())
        {
            throw new InvalidOperationException($"Failed to extract fingerprint with standard output {fingerprint} and diagnostic information {diagnostics}");
        }
        logger.ExtractedFingerprint(fingerprint);
        return fingerprint;
    }

    private async Task<(bool Success, string Output, string Diagnostics)> ExecuteCommand(
        string command,
        string? input = null
    )
    {
        logger.ExecuteCommand(command);
        var escapedCommand = command.Replace("\"", "\\\"");
        var process = Process.Start(
            new ProcessStartInfo()
            {
                FileName = "bash",
                Arguments = $"-c \"{escapedCommand}\"",
                ErrorDialog = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        ) ?? throw new InvalidOperationException($"The process for command '${command}' with the input '${input}' failed to start.");
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
        return (process.ExitCode == 0, output, diagnostics);
    }
}