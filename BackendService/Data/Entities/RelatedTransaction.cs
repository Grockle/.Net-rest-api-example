namespace BackendService.Data.Entities
{
    public class RelatedTransaction : AuditableBaseEntity
    {
        public int RelatedUserId { get; set; }
        public int GroupId { get; set; }
        public double RelatedAmount { get; set; }
        public int TransactionId { get; set; }
    }
}
