using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Group.Request
{
    public class AddGroupRequest
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [MinLength(4)]
        public string GroupName { get; set; }
        
        [Required]
        [MinLength(20)]
        public string Description { get; set; }
    }
}