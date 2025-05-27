using System.Threading.Tasks;

namespace Database.Services.Interfaces;

public interface IEmailSender
{
    public Task SendAsync(
        (string name, string address) recipient,
        string subject,
        string body
    );
}