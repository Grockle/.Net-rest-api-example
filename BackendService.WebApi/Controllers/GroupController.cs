using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Models.Requests.Group;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.Group;
using BackendService.Application.Models.Responses.User;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    public class GroupController : BaseApiController
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("JoinRequests")]
        public async Task<ActionResult<IEnumerable<GetGroupJoinResponse>>> JoinRequestsAsync(string shareCode)
        {
            var response = await _groupService.JoinRequestsAsync(shareCode);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response.Data);
        }

        [HttpGet("Users")]
        public async Task<ActionResult<IEnumerable<GetGroupUsersInfoResponse>>> UsersAsync(int groupId)
        {
            var response = await _groupService.UsersAsync(groupId);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }

        [HttpGet("Detail")]
        public async Task<ActionResult<GetGroupDetailResponse>> GetGroupDetailAsync(string shareCode)
        {
            var response = await _groupService.DetailAsync(Token, shareCode);
           
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }
            
            return Ok(response.Data);
        }

        [HttpGet("Details")]
        public async Task<ActionResult<List<GetGroupDetailResponse>>> GroupsAsync()
        {
            var response = await _groupService.DetailsAsync(Token);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }

        [HttpPost("Add")]
        public async Task<BaseResponse<bool>> AddGroupAsync(AddGroupRequest model)
        {
            return await _groupService.AddAsync(model);
        }
        
        [HttpPost("Category/Add")]
        public async Task<ActionResult<bool>> AddCategoryAsync(AddGroupCategoryRequest model)
        {
            var response = await _groupService.AddCategoryAsync(model, Token);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}