namespace BackendService.Data.Entities
{
    public class Transaction : AuditableBaseEntity
    {
        public int GroupId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
