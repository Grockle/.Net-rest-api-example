using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Group
{
    public class AddGroupCategoryRequest
    {
        [Required]
        public string GroupShareCode { get; set; }
        
        [Required]
        public int Type { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}