namespace BackendService.Domain.Entity
{
    public class RelatedTransaction : AuditableBaseEntity
    {
        public int RelatedUserId { get; set; }
        public int TransactionId { get; set; }
    }
}
