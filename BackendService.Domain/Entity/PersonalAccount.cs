namespace BackendService.Domain.Entity
{
    public class PersonalAccount : AuditableBaseEntity
    {
        public int UserId { get; set; }
        public double Amount { get; set; }
        public string CategoryName { get; set; }
    }
}