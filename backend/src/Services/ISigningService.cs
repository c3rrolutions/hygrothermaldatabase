using System.Threading.Tasks;

namespace Database.Services;

/// <summary>
/// Service to sign data string with GNUPG.
/// </summary>
public interface ISigningService
{
    /// <summary>
    /// Initialize service by importing private key from file passed into container.
    /// </summary>
    /// <returns> True, if privat key was successfully imported and fingerprint set. </returns>
    public Task<bool> Initialize();

    /// <summary>
    /// Get fingerprint
    /// </summary>
    /// <returns> Fingerprint as string </returns>
    public string GetFingerprint();

    /// <summary>
    /// Sign passed data string and return signature.
    /// </summary>
    /// <param name="data"> Data string to create signature from. </param>
    /// <returns> True and generated signature, if successful. Otherwise false. </returns>
    public Task<(bool, string)> SignData(string data);
}