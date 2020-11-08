using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.User.Request;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;
using BackendService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private IConfiguration _config;  
        public AccountController(IAccountService accountService, IConfiguration config)
        {
            _accountService = accountService;
            _config = config;  
        }
        
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult > Authenticate(LoginRequest userParam)
        {
            var user = await _accountService.Authenticate(userParam.Email, userParam.Password);
            if (user == null)
                return BadRequest(new { message = "Kullanici veya şifre hatalı!" });
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<BaseResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest registerModel)
        {
            return await _accountService.RegisterAsync(registerModel);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<BaseResponse<LoginResponse>> ConfirmEmailAsync(ConfirmEmailRequest confirmEmailRequest)
        {
            return await _accountService.ConfirmEmail(confirmEmailRequest);
        }

        [HttpPost("UpdateVerificationCode")]
        public async Task<BaseResponse<bool>> UpdateVerificationCodeAsync(UpdateEmailVerificationCodeRequest updateRequest)
        {
            return await _accountService.UpdateVerificationCodeAsync(updateRequest);
        }
        
        [HttpPost("Login")]
        public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            return await _accountService.LoginAsync(loginRequest);
        }
        
        [HttpPost("LoginWithToken")]
        public async Task<BaseResponse<LoginResponse>> LoginWithTokenAsync(string token)
        {
            return await _accountService.LoginWithTokenAsync(token);
        }
        
        [HttpPost("ControlResetCode")]
        public async Task<BaseResponse<bool>> ControlResetCodeAsync(string email, string resetCode)
        {
            return await _accountService.ControlEmailResetCodeAsync(email, resetCode);
        }
        
        [HttpPost("ResetPasswordRequest")]
        public async Task<BaseResponse<bool>> ResetPasswordRequestAsync(string email)
        {
            return await _accountService.SendPasswordResetRequestAsync(email);
        }
        
        [HttpPost("ChangePassword")]
        public async Task<BaseResponse<bool>> ChangePasswordAsync(ResetPasswordRequest model)
        {
            return await _accountService.ChangePasswordAsync(model);
        }

    }
}