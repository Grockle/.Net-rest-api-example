using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Models.Requests.User;
using BackendService.Application.Models.Responses.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
  
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
 
        }
        
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<ActionResult<LoginResponse>> Authenticate(LoginRequest userParam)
        {
            var response = await _accountService.Authenticate(userParam.Email, userParam.Password);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            if (response.Data == null)
            {
                return BadRequest(new { message = "Wrong Email or Password" });
            }
            
            return Ok(response.Data);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<RegisterUserResponse>> RegisterAsync(RegisterUserRequest registerModel)
        {
            var response = await _accountService.RegisterAsync(registerModel);
            
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