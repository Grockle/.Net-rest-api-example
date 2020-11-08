using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.User.Request
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