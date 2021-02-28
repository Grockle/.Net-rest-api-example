using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Group
{
    public class AddGroupRequest
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [MinLength(4)]
        public string GroupName { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        [MinLength(3)]
        [MaxLength(3)]
        public string MoneyShortCut { get; set; }
        
        [Required]
        public double Budget { get; set; }
    }
}