using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Database.Logging;
using Microsoft.Extensions.Logging;

namespace Database.Services;

public sealed class SigningService(
    AppSettings appSettings,
    ILogger<SigningService> logger)
{
    public async Task Initialize()
    {
        var (success, output, diagnostics) = await ExecuteCommand(
            $"gpg --batch --passphrase '{appSettings.GnupgSecretSigningKey.Passphrase}' --import './gpg-keys/{appSettings.GnupgSecretSigningKey.FileName}'"
        );
        if (!success)
        {
            throw new InvalidOperationException($"Failed to import GnuPG secret key for signing with passphrase '{appSettings.GnupgSecretSigningKey.Passphrase}' and file name '{appSettings.GnupgSecretSigningKey.FileName}'. The command gave the standard output '{output}' and the diagnostic information '{diagnostics}'.");
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
        return (process.ExitCode == 0, output, diagnostics);
    }
}