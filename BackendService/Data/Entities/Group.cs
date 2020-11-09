namespace BackendService.Data.Entities
{
    public class Group : AuditableBaseEntity
    {
        public string ShareCode { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string MoneyType { get; set; }
    }
}