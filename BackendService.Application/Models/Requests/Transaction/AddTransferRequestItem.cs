namespace BackendService.Application.Models.Requests.Transaction
{
    public class AddTransferRequestItem
    {
        public int RelatedUserId { get; set; }
        public double Amount { get; set; }
    }
}