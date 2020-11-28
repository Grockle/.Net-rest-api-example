using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Group.Request;
using BackendService.Data.DTOs.Group.Response;
using BackendService.Data.DTOs.User.Response;

namespace BackendService.Services
{
    public interface IGroupService
    {
        Task<BaseResponse<bool>> AddGroupAsync(AddGroupRequest model);
        Task<BaseResponse<IEnumerable<GetUserGroupsDto>>> GetUserGroupsAsync(int userId);
        Task<BaseResponse<IEnumerable<GetGroupJoinRequestsDto>>> GetGroupJoinRequests(string shareCode);
        Task<BaseResponse<bool>> SendGroupJoinRequest(int userId, string shareCode);
        Task<BaseResponse<bool>> ReplyGroupJoinRequestAsync(int requestId, int groupId, int adminId, bool isApproved);
        Task<BaseResponse<IEnumerable<GetGroupUsersInfoDto>>> GetGroupUsers(int groupId);
        Task<BaseResponse<List<GetGroupDetailDto>>> GetGroupDetailsAsync(string token);
    }
}