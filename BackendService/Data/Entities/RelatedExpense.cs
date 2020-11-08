namespace BackendService.Data.Entities
{
    public class RelatedExpense : AuditableBaseEntity
    {
        public int RelatedUserId { get; set; }
        public int GroupId { get; set; }
        public int ExpenseId { get; set; }
        public double RelatedAmount { get; set; }
    }
}
