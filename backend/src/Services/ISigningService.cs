using System.Threading.Tasks;

namespace Database.Services;

public interface ISigningService
{
    public Task<bool> ImportPrivateKey();

    public string GetFingerprint();

    public Task<(bool, string)> SignData(string data);
}