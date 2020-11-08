using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.User.Request
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