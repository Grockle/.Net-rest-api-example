using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Interface.Helper;
using BackendService.Application.Models.Requests.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    public class PasswordController : BaseApiController
    {
        private readonly IPasswordService _passwordService;

        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [AllowAnonymous]
        [HttpPost("Control")]
        public async Task<ActionResult<bool>> ControlResetCodeAsync(string email, string resetCode)
        {
            var response = await _passwordService.ControlAsync(email, resetCode);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }
            
            return Ok(response.Data);
        }
        
        [AllowAnonymous]
        [HttpPost("Reset")]
        public async Task<ActionResult<bool>> ResetPasswordRequestAsync(string email)
        {
            var response = await _passwordService.ResetAsync(email);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }
            
            return Ok(response.Data);
            
        }
        
        [AllowAnonymous]
        [HttpPost("Change")]
        public async Task<ActionResult<bool>> ChangePasswordAsync(ResetPasswordRequest model)
        {
            var response = await _passwordService.ChangeAsync(model);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }
            
            return Ok(response.Data);
        }
    }
}
