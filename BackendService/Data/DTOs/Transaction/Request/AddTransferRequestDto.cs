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
        public List<TransferDto> RelatedUsers { get; set; }
        
        [Required]
        public string TransactionType { get; set; }
        
        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Description { get; set; }
    }
}