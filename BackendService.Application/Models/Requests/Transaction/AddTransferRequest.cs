using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Transaction
{
    public class AddTransferRequest
    {
        [Required]
        public int WhoAdded { get; set; }
        
        [Required]
        public int GroupId { get; set; }

        [Required]
        public IEnumerable<AddTransferRequestItem> TransactionRelatedUsers { get; set; }

        [Required]
        public string Description { get; set; }
    }
}