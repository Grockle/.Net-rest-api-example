using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.User
{
    public class ConfirmEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Code { get; set; }
    }
}