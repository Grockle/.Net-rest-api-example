using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.User.Request
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Code { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        [PasswordPropertyText]
        public string Password { get; set; }
        
        [Required]
        [MinLength(6)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}