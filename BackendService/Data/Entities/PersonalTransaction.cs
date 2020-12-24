using System;

namespace BackendService.Data.Entities
{
    public class PersonalTransaction : AuditableBaseEntity
    {
        public int UserId { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionTime { get; set; }
        public int AccountId { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }
}