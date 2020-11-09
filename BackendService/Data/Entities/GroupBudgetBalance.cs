namespace BackendService.Data.Entities
{
    public class GroupBudgetBalance : AuditableBaseEntity
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public decimal Balance { get; set; }
    }
}