using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Transaction.Request
{
    public class AddTransferRequestDto
    {
        [Required]
        public int WhoAdded { get; set; }
        
        [Required]
        public int GroupId { get; set; }

        [Required]
        public IEnumerable<TransferDto> RelatedUsers { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Description { get; set; }
    }
}