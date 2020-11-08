namespace BackendService.Data.Entities
{
    public class GroupJoinRequest : AuditableBaseEntity
    {
        public int FromUserId { get; set; }
        public bool IsActive { get; set; }
        public string GroupShareCode { get; set; }
    }
}
