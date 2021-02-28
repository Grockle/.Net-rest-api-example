namespace BackendService.Domain.Entity
{
    public class GroupCategory : AuditableBaseEntity
    {
        public int GroupId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
    }
}