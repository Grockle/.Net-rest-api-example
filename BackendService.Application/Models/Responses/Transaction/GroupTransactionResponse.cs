using System;

namespace BackendService.Application.Models.Responses.Transaction
{
    public class GroupTransactionResponse
    {
        public int TransactionId { get; set; }
        public int AddedBy { get; set; }
        public int RelatedUserId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime CreateTime { get; set; }
    }
}