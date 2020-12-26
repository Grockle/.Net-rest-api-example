using System.Collections.Generic;

namespace BackendService.Data.DTOs.Transaction.Response
{
    public class GroupExpenseDto
    {
        public double Total { get; set; }
        public List<ExpenseGroupInfoDto> GroupedExpenses { get; set; }
    }
}