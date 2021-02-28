using System.Threading.Tasks;
using BackendService.Application.Models.Responses.General;

namespace BackendService.Application.Interface
{
    public interface IJoinRequestService
    {
        Task<BaseResponse<bool>> SendAsync(int userId, string shareCode);
        Task<BaseResponse<bool>> ReplyAsync(int requestId, int groupId, int adminId, bool isApproved);
    }
}
