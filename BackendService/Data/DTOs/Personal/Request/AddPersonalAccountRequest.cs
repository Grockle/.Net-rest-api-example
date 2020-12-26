using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Personal.Request
{
    public class AddPersonalAccountRequest
    {
        [Required]
        public double Amount { get; set; }
        
        [Required]
        public string CategoryName { get; set; }
    }
}