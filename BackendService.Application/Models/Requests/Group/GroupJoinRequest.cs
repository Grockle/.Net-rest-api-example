using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Group
{
    public class GroupJoinsRequest
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public string ShareCode { get; set; }
    }
}