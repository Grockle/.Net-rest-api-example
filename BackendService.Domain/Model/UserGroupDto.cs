using System;
using System.Collections.Generic;
using System.Text;

namespace BackendService.Domain.Model
{
    public class UserGroupDto
    {
        public int GroupId { get; set; }
        public string GroupShareCode { get; set; }
        public string GroupName { get; set; }
        public int AdminId { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public double Budget { get; set; }
    }
}
