using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Group.Request
{
    public class GroupJoinRequestDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public string ShareCode { get; set; }
    }
}