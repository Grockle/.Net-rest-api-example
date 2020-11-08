using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.DTOs.Expense.Request
{
    public class AddExpensiveRequestDto
    {
        [Required]
        public int WhoAdded { get; set; }
        
        [Required]
        public int GroupId { get; set; }
        
        [Required]
        public double Amount { get; set; }
        
        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Description { get; set; }

        [Required]
        public List<int> RelatedUserIds { get; set; }
    }
}