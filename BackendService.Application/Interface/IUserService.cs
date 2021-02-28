using System.Threading.Tasks;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.User;

namespace BackendService.Application.Interface
{
    public interface IUserService
    {
        Task<BaseResponse<UserInfoResponse>> InfoAsync(string token);
    }
}
