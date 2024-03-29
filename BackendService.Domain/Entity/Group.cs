﻿namespace BackendService.Domain.Entity
{
    public class Group : AuditableBaseEntity
    {
        public string ShareCode { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string MoneyType { get; set; }
        public double Budget { get; set; }
    }
}