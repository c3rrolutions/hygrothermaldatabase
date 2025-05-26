using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Database.Logging;
using Microsoft.Extensions.Logging;

namespace Database.Services;

/// <summary>
/// Implementation of <see cref="ISigningService"/>
/// </summary>
/// <param name="appSettings"> <see cref="AppSettings"/> </param>
/// <param name="logger">      Instance of <see cref="ILogger"/> </param>
public class SigningService(
    AppSettings appSettings,
    ILogger<ISigningService> logger) : ISigningService
{
    private const string FILENAME = "dataToSign";
    private const string FILE_EXTENSION = ".asc";

    /// <inheritdoc/>
    public string Fingerprint { get; private set; } = "";

    /// <inheritdoc/>
    public async Task Initialize()
    {
        // Execute command to import private key
        var (success, outout) = await ExecuteGnuCommand($"gpg --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --import ../gpg-keys/{appSettings.GnupgPrivateKeyFilename}");
        if (success)
        {
            // Extract fingerprint
            Fingerprint = await GetFingerprintFromKeyList();
            if (!String.IsNullOrEmpty(Fingerprint))
            {
                logger.Fingerprint(Fingerprint);
            }
            else
            {
                logger.FingerprintError();
                throw new InvalidOperationException("Failed to initialize GNUPG signing service.");
            }
        }
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string Output)> SignData(string data)
    {
        // Write data string to file
        if (!WriteDataToFile(data))
        {
            return (false, "");
        }

        // Execute command to generate detached signature file Creates file with .asc ending
        var (success, outout) = await ExecuteGnuCommand($"gpg --pinentry-mode loopback --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --detach-sig --armor --local-user {Fingerprint} {FILENAME}");

        if (success)
        {
            // Read signature from file
            (success, outout) = ReadSignatureFromFile();
            logger.Signature(outout);
        }
        else
        {
            logger.SignatureError(outout);
        }

        // Remove created files
        RemoveFiles();
        return (success, outout);
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
        string output = proc.StandardOutput.ReadToEnd();
        string error = proc.StandardError.ReadToEnd();
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

    private async Task<string> GetFingerprintFromKeyList()
    {
        string fingerprint = "";

        // Execute command to list keys with fingerprint, use 'with-colon' to split later
        var (success, outout) = await ExecuteGnuCommand($"gpg --list-keys --with-subkey-fingerprint --with-colon");
        if (success)
        {
            var splitedOutput = outout.Split(':');
            fingerprint = splitedOutput[splitedOutput.Length - 2];
        }
        return fingerprint;
    }

    private bool WriteDataToFile(string data)
    {
        try
        {
            using (StreamWriter outputFile = new StreamWriter(FILENAME))
            {
                outputFile.WriteLine(data);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.WriteFileException(ex.ToString());
            return false;
        }
    }

    private (bool, string) ReadSignatureFromFile()
    {
        try
        {
            using (StreamReader file = new StreamReader(FILENAME + FILE_EXTENSION))
            {
                return (true, file.ReadToEnd());
            }
        }
        catch (IOException ex)
        {
            logger.ReadFileException(ex.ToString());
            return (false, ex.ToString());
        }
    }

    private static void RemoveFiles()
    {
        File.Delete(FILENAME);
        File.Delete(FILENAME + FILE_EXTENSION);
    }
}