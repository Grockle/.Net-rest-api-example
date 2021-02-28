using System.Collections.Generic;

namespace BackendService.Application.Models.Responses.Personal
{
    public class GroupedPersonalCategoryResponse
    {
        public List<GroupedPersonalCategoryResponseItem> ExpenseCategories { get; set; }
        public List<GroupedPersonalCategoryResponseItem> IncomeCategories { get; set; }
        public List<GroupedPersonalCategoryResponseItem> AccountCategories { get; set; }
    }
}