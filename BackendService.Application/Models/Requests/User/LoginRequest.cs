using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.User
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}