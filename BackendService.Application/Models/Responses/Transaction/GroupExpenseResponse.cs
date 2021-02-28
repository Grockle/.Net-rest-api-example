using System.Collections.Generic;

namespace BackendService.Application.Models.Responses.Transaction
{
    public class GroupExpenseResponse
    {
        public double Total { get; set; }
        public List<GroupExpenseResponseItem> GroupedExpenses { get; set; }
    }
}