using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Personal
{
    public class UpdatePersonalCategoryRequest
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public int Type { get; set; }
        
        [Required]
        public string CategoryName { get; set; }
    }
}