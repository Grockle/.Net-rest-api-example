using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Group.Request
{
    public class ReplyGroupJoinRequestDto
    {
        [Required]
        public int AdminId { get; set; }
        
        [Required]
        public int RequestId { get; set; }
        
        [Required]
        public int GroupId { get; set; }
        
        [Required]
        public bool IsApproved { get; set; }
    }
}