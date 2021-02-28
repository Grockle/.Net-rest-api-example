using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Models.Requests.User;
using BackendService.Application.Models.Responses.User;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    public class EmailVerificationController : BaseApiController
    {
        private readonly IEmailVerificationService _emailVerificationService;

        public EmailVerificationController(IEmailVerificationService emailVerificationService)
        {
            _emailVerificationService = emailVerificationService;
        }

        [HttpPost("Confirm")]
        public async Task<ActionResult<LoginResponse>> ConfirmAsync(ConfirmEmailRequest confirmEmailRequest)
        {
            var response = await _emailVerificationService.ConfirmAsync(confirmEmailRequest, Token);

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

        [HttpPost("Update")]
        public async Task<ActionResult<bool>> UpdateAsync(UpdateEmailVerificationCodeRequest updateRequest)
        {
            var response = await _emailVerificationService.UpdateAsync(updateRequest);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}
