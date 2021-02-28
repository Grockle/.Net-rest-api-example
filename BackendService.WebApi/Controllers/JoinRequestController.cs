using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Models.Requests.Group;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    public class JoinRequestController : BaseApiController
    {
        private readonly IJoinRequestService _joinRequestService;

        public JoinRequestController(IJoinRequestService joinRequestService)
        {
            _joinRequestService = joinRequestService;
        }

        [HttpPost("Send")]
        public async Task<ActionResult<bool>> SendGroupJoinRequestAsync(GroupJoinsRequest model)
        {
            var response = await _joinRequestService.SendAsync(model.UserId, model.ShareCode);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }

        [HttpPost("Reply")]
        public async Task<ActionResult<bool>> ReplyAsync(ReplyGroupJoinRequest model)
        {
            var response = await _joinRequestService.ReplyAsync(model.RequestId, model.GroupId, model.AdminId, model.IsApproved);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}
