using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Database.Services;

public class SigningService(
    AppSettings appSettings,
    ILogger<ISigningService> logger) : ISigningService
{
    private string _fingerprint = "";

    public async Task<bool> ImportPrivateKey()
    {
        var (success, outout) = await ExecuteGnuCommand($"gpg --batch --passphrase Freiburg2005 --import ../gpg-keys/{appSettings.GnupgPrivateKeyFilename}");
        if (success)
        {
            _fingerprint = await GetFingerprint();
            if (!String.IsNullOrEmpty(_fingerprint))
            {
                logger.LogDebug("GNUPG Fingerprint: {Fingerprint}", _fingerprint);
            }
            else
            {
                logger.LogError("GNUPG Error: Failed to get fingerprint.");
                success = false;
            }
        }
        return success;
    }

    public string GetFingerprint()
    {
        return _fingerprint;
    }

    public async Task<(bool, string)> SignData(string data)
    {
        var filepath = WriteDataToFile(data);
        var (success, outout) = await ExecuteGnuCommand($"gpg --pinentry-mode loopback --batch --passphrase {appSettings.GnupgPrivateKeyPassphrase} --detach-sig --armor --local-user {_fingerprint} {Path.GetFileName(filepath)}");
        string signature = "";

        if (success)
        {
            success = ReadSignatureFromFile(filepath, out signature);
            logger.LogDebug("GNUPG Signature: {Signature}", signature);
        }
        else
        {
            logger.LogError("GNUPG Error: {Outout}", outout);
        }
        RemoveFiles(filepath);
        return (success, signature);
    }

    private async Task<(bool, string)> ExecuteGnuCommand(string command)
    {
        logger.LogDebug("GNUPG: Execute command. ({Command})", command);

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
            logger.LogDebug("GNUPG Error: {Error}", error);
        }
        logger.LogDebug("GNUPG Output: {Output}", output);
        logger.LogDebug("GNUPG ExitCode: {ExitCode}", proc.ExitCode);
        return (proc.ExitCode == 0, output);
    }

    private async Task<string> GetFingerprint()
    {
        string fingerprint = "";
        var (success, outout) = await ExecuteGnuCommand($"gpg --list-keys --with-subkey-fingerprint --with-colon");
        if (success)
        {
            var splitedOutput = outout.Split(':');
            fingerprint = splitedOutput[splitedOutput.Length - 2];
        }
        return fingerprint;
    }

    private static string WriteDataToFile(string data)
    {
        string docPath = Path.Combine(Environment.CurrentDirectory, "dataToSign.json");

        using (StreamWriter outputFile = new StreamWriter(docPath))
        {
            outputFile.WriteLine(data);
        }

        return docPath;
    }

    private static bool ReadSignatureFromFile(string filepath, out string signature)
    {
        try
        {
            using (StreamReader file = new StreamReader(filepath + ".asc"))
            {
                signature = file.ReadToEnd();
            }
            return true;
        }
        catch
        {
            signature = "";
            return false;
        }
    }

    private static void RemoveFiles(string filepath)
    {
        File.Delete(filepath);
        File.Delete(filepath + ".asc");
    }
}