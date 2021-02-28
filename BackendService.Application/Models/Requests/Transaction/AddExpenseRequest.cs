using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Application.Models.Requests.Transaction
{
    public class AddExpenseRequest
    {
        [Required]
        public int WhoAdded { get; set; }
        
        [Required]
        public int GroupId { get; set; }
        
        [Required]
        public double Amount { get; set; }
        
        [Required]
        public string Description { get; set; }

        [Required]
        public IEnumerable<int> RelatedUserIds { get; set; }

        [Required]
        public string Category { get; set; }
    }
}