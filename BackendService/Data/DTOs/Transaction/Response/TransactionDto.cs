using System;
using System.Collections.Generic;
using BackendService.Data.DTOs.User.Response;

namespace BackendService.Data.DTOs.Transaction.Response
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int AdderId {get; set; }
        public string AdderName { get; set; }
        public string AdderSurname { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime CreateTime { get; set; }
        public List<RelatedUserDto> RelatedUsers { get; set; }
    }
}