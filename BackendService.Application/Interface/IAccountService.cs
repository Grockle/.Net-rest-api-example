using System.Threading.Tasks;
using BackendService.Application.Models.Requests.User;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.User;

namespace BackendService.Application.Interface
{
    public interface IAccountService
    {
        Task<BaseResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest userModel);
        Task<BaseResponse<LoginResponse>> Authenticate(string email, string password);
    }
}