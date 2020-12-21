using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.User.Request;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;

namespace BackendService.Services
{
    public interface IAccountService
    {
        Task<BaseResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest userModel);
        Task<BaseResponse<LoginResponse>> ConfirmEmail(ConfirmEmailRequest confirmModel, string token);
        Task<BaseResponse<bool>> UpdateVerificationCodeAsync(UpdateEmailVerificationCodeRequest model);
        Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest loginModel);
        Task<BaseResponse<bool>> ControlResetCodeAsync(string email, string resetCode);
        Task<BaseResponse<bool>> SendPasswordResetRequestAsync(string email);
        Task<BaseResponse<bool>> ChangePasswordAsync(ResetPasswordRequest resetRequestModel);
        Task<BaseResponse<LoginResponse>> Authenticate(string email, string password);
        Task<BaseResponse<UserInfoDto>> GetUserInfoAsync(string token);
    }
}