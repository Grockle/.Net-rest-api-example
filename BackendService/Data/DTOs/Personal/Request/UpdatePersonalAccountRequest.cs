using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Personal.Request
{
    public class UpdatePersonalAccountRequest
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public double Amount { get; set; }
        
        [Required]
        public string CategoryName { get; set; }
    }
}