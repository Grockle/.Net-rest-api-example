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
        public IEnumerable<TransferDto> TransactionRelatedUsers { get; set; }

        [Required]
        public string Description { get; set; }
    }
}