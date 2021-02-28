namespace BackendService.Domain.Entity
{
    public class PersonalCategory : AuditableBaseEntity
    {
        public int UserId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
    }
}