using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.User
{
    public class UpdateEmailVerificationCodeRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Wrong email format.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}