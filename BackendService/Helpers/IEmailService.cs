using System.Threading.Tasks;
using BackendService.Models;

namespace BackendService.Helpers
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}
