using System.Threading.Tasks;
using BackendService.Application.Models.Requests.General;

namespace BackendService.Application.Interface.Helper
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}
