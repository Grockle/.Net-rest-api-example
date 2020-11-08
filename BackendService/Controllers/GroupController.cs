using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Group.Request;
using BackendService.Data.DTOs.Group.Response;
using BackendService.Data.DTOs.User.Response;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        
        [HttpPost("Add")]
        public async Task<BaseResponse<bool>> AddGroupAsync(AddGroupRequest model)
        {
            return await _groupService.AddGroupAsync(model.UserId, model.GroupName, model.Description);
        }
        
        [HttpGet("GetGroups")]
        public async Task<BaseResponse<List<GetUserGroupsDto>>> GetGroupsAsync(int userId)
        {
            return await _groupService.GetUserGroupsAsync(userId);
        }

        [HttpGet("GetJoinRequests")]
        public async Task<BaseResponse<List<GetGroupJoinRequestsDto>>> GetJoinRequestsAsync(string shareCode)
        {
            return await _groupService.GetGroupJoinRequests(shareCode);
        }
        
        [HttpPost("SendJoinRequest")]
        public async Task<BaseResponse<bool>> SendGroupJoinRequestAsync(GroupJoinRequestDto model)
        {
            return await _groupService.SendGroupJoinRequest(model.UserId, model.ShareCode);
        }
        
        [HttpPost("ReplyJoinRequest")]
        public async Task<BaseResponse<bool>> ReplyJoinRequestAsync(ReplyGroupJoinRequestDto model)
        {
            return await _groupService.ReplyGroupJoinRequestAsync(model.RequestId, model.GroupId, model.AdminId, model.IsApproved);
        }
        
        [HttpGet("GetGroupUsers")]
        public async Task<BaseResponse<List<GetGroupUsersInfoDto>>> GetGroupUsersAsync(int groupId)
        {
            return await _groupService.GetGroupUsers(groupId);
        }
    }
}