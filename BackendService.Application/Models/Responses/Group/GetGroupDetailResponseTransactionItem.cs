using System;
using System.Collections.Generic;
using BackendService.Application.Models.Responses.Transaction;

namespace BackendService.Application.Models.Responses.Group
{
    public class GetGroupDetailResponseTransactionItem
    {
        public int TransactionId { get; set; }
        public int AdderId {get; set; }
        public string AdderName { get; set; }
        public string AdderSurname { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime CreateTime { get; set; }
        public List<TransactionResponseItem> RelatedUsers { get; set; }
    }
}