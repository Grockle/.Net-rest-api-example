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
            return await _groupService.AddGroupAsync(model);
        }
        
        [HttpGet("GetGroups")]
        public async Task<ActionResult<IEnumerable<GetUserGroupsDto>>> GetGroupsAsync(int userId)
        {
            var response = await _groupService.GetUserGroupsAsync(userId);

            if (response.HasError)
            {
                return BadRequest(response.Message);
            }

            if (response.Data == null)
            {
                return NotFound();
            }
            
            return Ok(response.Data);
        }

        [HttpGet("GetJoinRequests")]
        public async Task<ActionResult<IEnumerable<GetGroupJoinRequestsDto>>> GetJoinRequestsAsync(string shareCode)
        {
            var response = await _groupService.GetGroupJoinRequests(shareCode);

            if (response.HasError)
            {
                return BadRequest(response.Message);
            }

            if (response.Data == null)
            {
                return NotFound();
            }
            
            return Ok(response.Data);
        }
        
        [HttpPost("SendJoinRequest")]
        public async Task<ActionResult<bool>> SendGroupJoinRequestAsync(GroupJoinRequestDto model)
        {
            var response = await _groupService.SendGroupJoinRequest(model.UserId, model.ShareCode);

            if (response.HasError)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }
        
        [HttpPost("ReplyJoinRequest")]
        public async Task<ActionResult<bool>> ReplyJoinRequestAsync(ReplyGroupJoinRequestDto model)
        {
            var response = await _groupService.ReplyGroupJoinRequestAsync(model.RequestId, model.GroupId, model.AdminId, model.IsApproved);

            if (response.HasError)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }
        
        [HttpGet("GetGroupUsers")]
        public async Task<ActionResult<IEnumerable<GetGroupUsersInfoDto>>> GetGroupUsersAsync(int groupId)
        {
            var response = await _groupService.GetGroupUsers(groupId);

            if (response.HasError)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }
    }
}