using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.User.Request
{
    public class AuthenticateRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}