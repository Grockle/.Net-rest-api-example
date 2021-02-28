using System.Threading.Tasks;
using BackendService.Application.Models.Requests.User;
using BackendService.Application.Models.Responses.General;

namespace BackendService.Application.Interface
{
    public interface IPasswordService
    {
        Task<BaseResponse<bool>> ControlAsync(string email, string resetCode);
        Task<BaseResponse<bool>> ResetAsync(string email);
        Task<BaseResponse<bool>> ChangeAsync(ResetPasswordRequest resetRequestModel);
    }
}
