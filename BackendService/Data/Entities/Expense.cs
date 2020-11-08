namespace BackendService.Data.Entities
{
    public class Expense : AuditableBaseEntity
    {
        public int GroupId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
    }
}
