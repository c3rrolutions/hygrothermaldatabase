using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Utilities;

public static class Sha256FileHasher
{
    public static async Task<string> Compute(string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.", filePath);
        }
        using var sha256 = SHA256.Create();
        using var fileStream = File.OpenRead(filePath);
        return Convert.ToHexString(
            await sha256.ComputeHashAsync(fileStream, cancellationToken)
        );
    }
}