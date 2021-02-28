using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Personal
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