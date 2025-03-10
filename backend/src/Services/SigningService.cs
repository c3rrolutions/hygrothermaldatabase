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
    private string _fingerprint = "";
    private const string FILENAME = "dataToSign";

    /// <inheritdoc/>
    public async Task<bool> Initialize()
    {
        // Execute command to import private key
        var (success, outout) = await ExecuteGnuCommand($"gpg --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --import ../gpg-keys/{appSettings.GnupgPrivateKeyFilename}");
        if (success)
        {
            // Extract fingerprint
            _fingerprint = await GetFingerprintFromKeyList();
            if (!String.IsNullOrEmpty(_fingerprint))
            {
                logger.Fingerprint(_fingerprint);
            }
            else
            {
                logger.FingerprintError();
                success = false;
            }
        }
        return success;
    }

    /// <inheritdoc/>
    public string GetFingerprint()
    {
        return _fingerprint;
    }

    /// <inheritdoc/>
    public async Task<(bool, string)> SignData(string data)
    {
        string signature = "";

        // Write data string to file
        if (!WriteDataToFile(data))
        {
            return (false, "");
        }

        // Execute command to generate detached signature file Creates file with .asc ending
        var (success, outout) = await ExecuteGnuCommand($"gpg --pinentry-mode loopback --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --detach-sig --armor --local-user {_fingerprint} {FILENAME}");

        if (success)
        {
            // Read signature from file
            success = ReadSignatureFromFile(out signature);
            logger.Signature(signature);
        }
        else
        {
            logger.SignatureError(outout);
        }

        // Remove created files
        RemoveFiles();
        return (success, signature);
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

    private bool ReadSignatureFromFile(out string signature)
    {
        try
        {
            using (StreamReader file = new StreamReader(FILENAME + ".asc"))
            {
                signature = file.ReadToEnd();
            }
            return true;
        }
        catch (Exception ex)
        {
            logger.ReadFileException(ex.ToString());
            signature = "";
            return false;
        }
    }

    private static void RemoveFiles()
    {
        File.Delete(FILENAME);
        File.Delete(FILENAME + ".asc");
    }
}