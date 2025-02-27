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
    //Export private subkey: gpg --output private.pgp --armor --export-secret-key C97074097892B88EF6D23BA9E5D03DA76EDC407D
    private string _fingerprint = "";

    public async Task<bool> ImportPrivateKey()
    {
        //await ExecuteCommand();
        var (success, outout) = await ExecuteGnuCommand($"gpg --batch --passphrase Freiburg2005 --import ../gpg-keys/{appSettings.GnupgPrivateKeyFilename}");
        if (success)
        {
            _fingerprint = await GetFingerprint();
            if (!String.IsNullOrEmpty(_fingerprint))
            {
                logger.LogDebug("GNUPG Fingerprint: {Fingerprint}", _fingerprint);
                return true;
            }
        }
        return false;
    }

    public async Task<bool> SignData(string data)
    {
        //await ExecuteCommand();
        //await ExecuteGnuCommand($"echo $(tty)");
        var filepath = WriteDataToFile(data);
        await ExecuteGnuCommand($"echo $(tty)");
        await ExecuteGnuCommand($"echo $GPG_TTY");
        await ExecuteGnuCommand($"gpgconf --list-dirs");
        // gpg --detach-sig --armor --local-user <SIGNING_KEY_FINGERPRINT> response.json
        //var (success, outout) = await ExecuteGnuCommand($"gpg --output {Path.GetFileName(filepath)}.asc --detach-sig --armor {Path.GetFileName(filepath)}");
        //var (success, outout) = await ExecuteGnuCommand($"gpg --batch --passphrase Freiburg2005 --detach-sig --armor --local-user {_fingerprint} {Path.GetFileName(filepath)}");
        await ExecuteGnuCommand($"echo \"test\" | gpg --batch --clearsign");
        var (success, outout) = await ExecuteGnuCommand($"gpg --batch --passphrase Freiburg2005 --clearsign {Path.GetFileName(filepath)}");

        var signature = ReadSignatureFromFile(filepath);
        logger.LogDebug("GNUPG Signature: {Signature}", signature);
        //RemoveFiles(filepath);
        return success;
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
            //WorkingDirectory = "/home/me/app/src/",
            //RedirectStandardInput = true,
            //UseShellExecute = false,
            //UserName = "me"
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
        //gpg--list - keys--with - subkey - fingerprint--with - colon
        //tru::1:1740060571:0:3:1:5
        //pub: -:2048:17:AF86639859F25224: 1739973559:1803045559::-:::scSC::::::23::0:
        //fpr:::::::::8CC878D9614150C8278C6F0FAF86639859F25224:
        //uid: -::::1739973559::90476D25C295C40CFD4D89EAB1BF3C879E39165B::James Bremer<jb@interap.de>::::::::::0:
        //sub: -:2048:17:E5D03DA76EDC407D: 1739973625:1803045625:::::s::::::23:
        //fpr:::::::::C97074097892B88EF6D23BA9E5D03DA76EDC407D:
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

    private static string ReadSignatureFromFile(string filepath)
    {
        string signature = "";
        using (StreamReader file = new StreamReader(filepath + ".asc"))
        {
            signature = file.ReadToEnd();
        }
        return signature;
    }

    private static void RemoveFiles(string filepath)
    {
        File.Delete(filepath);
        File.Delete(filepath + ".asc");
    }
}