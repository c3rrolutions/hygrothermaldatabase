using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Database.Logging;
using Database.Services;
using Microsoft.Extensions.Logging;

namespace Database.Services;

/// <summary>
/// Service to sign data string with GNUPG.
/// </summary>
/// <param name="appSettings"> <see cref="AppSettings"/> </param>
/// <param name="logger">      Instance of <see cref="ILogger"/> </param>
public sealed class SigningService(
    AppSettings appSettings,
    ILogger<SigningService> logger)
{
    private const string FILENAME = "dataToSign";
    private const string FILE_EXTENSION = ".asc";

    /// <summary>
    /// Initialize service by importing private key from file passed into container.
    /// </summary>
    /// <returns> True, if privat key was successfully imported and fingerprint set. </returns>
    /// <exception cref="InvalidOperationException"> If initialization failed. </exception>
    public async Task Initialize()
    {
        // Execute command to import private key
        var (success, output) = await ExecuteGnuCommand($"gpg --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --import ./gpg-keys/{appSettings.GnupgPrivateKeyFilename}");
        if (success)
        {
            var fingerprint = await GetFingerprint();
            if (!string.IsNullOrEmpty(fingerprint))
            {
                logger.Fingerprint(fingerprint);
            }
            else
            {
                logger.FingerprintError();
                throw new InvalidOperationException("Failed to initialize GnuPG signing service.");
            }
        }
    }

    /// <summary>
    /// Sign passed data string and return signature.
    /// </summary>
    /// <param name="data"> Data string to create signature from. </param>
    /// <returns> True and generated signature, if successful. Otherwise false and error. </returns>
    public async Task<(bool Success, string Output)> SignData(string data)
    {
        // Write data string to file
        if (!WriteDataToFile(data))
        {
            return (false, "");
        }

        // Execute command to generate detached signature file Creates file with .asc ending
        var (success, output) = await ExecuteGnuCommand($"gpg --pinentry-mode loopback --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --detach-sig --armor --local-user {await GetFingerprint()} {FILENAME}");

        if (success)
        {
            // Read signature from file
            (success, output) = ReadSignatureFromFile();
            logger.Signature(output);
        }
        else
        {
            logger.SignatureError(output);
        }

        // Remove created files
        RemoveFiles();
        return (success, output);
    }

    private async Task<(bool, string)> ExecuteGnuCommand(string command)
    {
        logger.ExecuteCommand(command);

        var escapedArgs = command.Replace("\"", "\\\"");
        ProcessStartInfo startInfo = new()
        {
            FileName = "bash",
            Arguments = $"-c \"{escapedArgs}\"",
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        var proc = Process.Start(startInfo);
        ArgumentNullException.ThrowIfNull(proc);
        var output = proc.StandardOutput.ReadToEnd();
        var error = proc.StandardError.ReadToEnd();
        await proc.WaitForExitAsync();
        if (string.IsNullOrEmpty(output))
        {
            output = error;
        }
        else if (!string.IsNullOrEmpty(error))
        {
            logger.ExecuteCommandError(error);
        }
        logger.ExecuteCommandOutput(output);
        logger.ExecuteCommandResult(proc.ExitCode);
        return (proc.ExitCode == 0, output);
    }

    /// <summary>
    /// Get fingerprint.
    /// </summary>
    /// <returns> The fingerprint. </returns>
    /// <exception cref="InvalidOperationException"> If fingerprint extraction failed. </exception>
    public async Task<string> GetFingerprint()
    {
        var (success, fingerprint) = await ExecuteGnuCommand("gpg --list-secret-keys --with-colons --keyid-format=long | awk -F: '$1==\"fpr\" {printf $10; exit}'");
        if (!success)
        {
            throw new InvalidOperationException("Failed to extract fingerprint.");
        }
        return fingerprint;
    }

    private bool WriteDataToFile(string data)
    {
        try
        {
            using (var outputFile = new StreamWriter(FILENAME))
            {
                outputFile.WriteLine(data);
            }

            return true;
        }
        catch (Exception exception)
        {
            logger.WriteFileException(exception.ToString());
            return false;
        }
    }

    private (bool, string) ReadSignatureFromFile()
    {
        try
        {
            using (var file = new StreamReader(FILENAME + FILE_EXTENSION))
            {
                return (true, file.ReadToEnd());
            }
        }
        catch (IOException exception)
        {
            logger.ReadFileException(exception.ToString());
            return (false, exception.ToString());
        }
    }

    private static void RemoveFiles()
    {
        File.Delete(FILENAME);
        File.Delete(FILENAME + FILE_EXTENSION);
    }
}