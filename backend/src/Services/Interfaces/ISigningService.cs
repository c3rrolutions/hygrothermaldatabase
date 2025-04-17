using System;
using System.Threading.Tasks;

namespace Database.Services.Interfaces;

/// <summary>
/// Service to sign data string with GNUPG.
/// </summary>
public interface ISigningService
{
    /// <summary>
    /// Get fingerprint
    /// </summary>
    /// <returns> Fingerprint as string </returns>
    public string Fingerprint { get; }

    /// <summary>
    /// Initialize service by importing private key from file passed into container.
    /// </summary>
    /// <returns> True, if privat key was successfully imported and fingerprint set. </returns>
    /// <exception cref="InvalidOperationException"> If initialization failed. </exception>
    public Task Initialize();

    /// <summary>
    /// Sign passed data string and return signature.
    /// </summary>
    /// <param name="data"> Data string to create signature from. </param>
    /// <returns> True and generated signature, if successful. Otherwise false and error. </returns>
    public Task<(bool Success, string Output)> SignData(string data);
}