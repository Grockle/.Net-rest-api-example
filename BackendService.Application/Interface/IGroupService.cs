using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Application.Models.Requests.Group;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.Group;
using BackendService.Application.Models.Responses.User;

namespace BackendService.Application.Interface
{
    public interface IGroupService
    {
        Task<BaseResponse<IEnumerable<GetGroupJoinResponse>>> JoinRequestsAsync(string shareCode);
        Task<BaseResponse<IEnumerable<GetGroupUsersInfoResponse>>> UsersAsync(int groupId);
        Task<BaseResponse<List<GetGroupDetailResponse>>> DetailsAsync(string token);
        Task<BaseResponse<GetGroupDetailResponse>> DetailAsync(string token, string shareCode);
        Task<BaseResponse<bool>> AddAsync(AddGroupRequest model);
    }
}