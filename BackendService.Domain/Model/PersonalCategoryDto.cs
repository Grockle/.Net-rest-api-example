using System;
using System.Collections.Generic;
using System.Text;

namespace BackendService.Domain.Model
{
    public class PersonalCategoryDto
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string CategoryName { get; set; }
    }
}
