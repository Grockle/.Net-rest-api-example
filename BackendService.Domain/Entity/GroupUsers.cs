namespace BackendService.Domain.Entity
{
    public class GroupUsers : AuditableBaseEntity
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}