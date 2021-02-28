using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Models.Responses.User;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Info")]
        public async Task<ActionResult<UserInfoResponse>> GetUserInfosAsync()
        {
            var response = await _userService.InfoAsync(Token);

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

    }
}
