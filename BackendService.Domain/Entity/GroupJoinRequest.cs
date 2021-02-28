namespace BackendService.Domain.Entity
{
    public class GroupJoinRequest : AuditableBaseEntity
    {
        public int FromUserId { get; set; }
        public bool IsActive { get; set; }
        public string GroupShareCode { get; set; }
    }
}
