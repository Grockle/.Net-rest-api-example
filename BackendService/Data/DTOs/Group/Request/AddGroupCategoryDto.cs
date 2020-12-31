using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Group.Request
{
    public class AddGroupCategoryDto
    {
        [Required]
        public string GroupShareCode { get; set; }
        
        [Required]
        public int Type { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}