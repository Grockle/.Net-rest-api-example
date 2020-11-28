using System.Threading.Tasks;
using BackendService.Data.DTOs.User.Request;
using BackendService.Data.DTOs.User.Response;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BackendService.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private IConfiguration _config;  
        public AccountController(IAccountService accountService, IConfiguration config)
        {
            _accountService = accountService;
            _config = config;  
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

        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<LoginResponse>> ConfirmEmailAsync(ConfirmEmailRequest confirmEmailRequest)
        {
            var response = await _accountService.ConfirmEmail(confirmEmailRequest);

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

        [HttpPost("UpdateVerificationCode")]
        public async Task<ActionResult<bool>> UpdateVerificationCodeAsync(UpdateEmailVerificationCodeRequest updateRequest)
        {
            var response = await _accountService.UpdateVerificationCodeAsync(updateRequest);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }

        [HttpPost("GetUserInfos")]
        public async Task<ActionResult<LoginResponse>> GetUserInfosAsync()
        {
            var response = await _accountService.LoginWithTokenAsync(Token);

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
        
        [HttpPost("ControlResetCode")]
        public async Task<ActionResult<bool>> ControlResetCodeAsync(string email, string resetCode)
        {
            var response = await _accountService.ControlResetCodeAsync(email, resetCode);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }
            
            return Ok(response.Data);
        }
        
        [HttpPost("ResetPasswordRequest")]
        public async Task<ActionResult<bool>> ResetPasswordRequestAsync(string email)
        {
            var response = await _accountService.SendPasswordResetRequestAsync(email);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }
            
            return Ok(response.Data);
            
        }
        
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<bool>> ChangePasswordAsync(ResetPasswordRequest model)
        {
            var response = await _accountService.ChangePasswordAsync(model);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }
            
            return Ok(response.Data);
        }

    }
}