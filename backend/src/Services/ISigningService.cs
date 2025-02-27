using System.Threading.Tasks;

namespace Database.Services;

public interface ISigningService
{
    public Task<bool> ImportPrivateKey();

    public Task<bool> SignData(string data);
}