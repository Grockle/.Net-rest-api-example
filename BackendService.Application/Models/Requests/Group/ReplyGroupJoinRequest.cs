using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Group
{
    public class ReplyGroupJoinRequest
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