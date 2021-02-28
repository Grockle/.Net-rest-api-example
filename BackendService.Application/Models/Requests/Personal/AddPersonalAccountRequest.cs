using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Personal
{
    public class AddPersonalAccountRequest
    {
        [Required]
        public double Amount { get; set; }
        
        [Required]
        public string CategoryName { get; set; }
    }
}