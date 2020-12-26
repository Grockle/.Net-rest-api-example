using System.Collections.Generic;

namespace BackendService.Data.DTOs.Personal.Response
{
    public class GroupedPersonalCategoryDto
    {
        public List<PersonalCategoryDto> ExpenseCategories { get; set; }
        public List<PersonalCategoryDto> IncomeCategories { get; set; }
        public List<PersonalCategoryDto> AccountCategories { get; set; }
    }
}