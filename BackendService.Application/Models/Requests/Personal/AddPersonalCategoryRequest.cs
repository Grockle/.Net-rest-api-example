using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Personal
{
    public class AddPersonalCategoryRequest
    {
        [Required]
        public int Type { get; set; }
        
        [Required]
        public string CategoryName { get; set; }
    }
}