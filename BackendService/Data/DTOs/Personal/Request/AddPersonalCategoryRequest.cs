using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Personal.Request
{
    public class AddPersonalCategoryRequest
    {
        [Required]
        public int Type { get; set; }
        
        [Required]
        public string CategoryName { get; set; }
    }
}