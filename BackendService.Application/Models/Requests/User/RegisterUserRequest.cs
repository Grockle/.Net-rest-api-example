using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.User
{
    public class RegisterUserRequest
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
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