using System.Threading.Tasks;
using BackendService.Application.Models.Requests.User;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.User;

namespace BackendService.Application.Interface
{
    public interface IEmailVerificationService
    {
        Task<BaseResponse<LoginResponse>> ConfirmAsync(ConfirmEmailRequest confirmModel, string token);
        Task<BaseResponse<bool>> UpdateAsync(UpdateEmailVerificationCodeRequest model);
    }
}
