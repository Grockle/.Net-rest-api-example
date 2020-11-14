namespace BackendService.Data.DTOs.Transaction.Request
{
    public class TransferDto
    {
        public int RelatedUserId { get; set; }
        public double Amount { get; set; }
    }
}