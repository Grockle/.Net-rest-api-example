using System.Collections.Generic;
using BackendService.Application.Models.Responses.Transaction;
using BackendService.Application.Models.Responses.User;

namespace BackendService.Application.Models.Responses.Group
{
    public class GetGroupDetailResponse
    {
        public int GroupId { get; set; }
        public int AdminId { get; set; }
        public string ShareCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public double Budget { get; set; }
        public List<GetGroupDetailResponseUserItem> UserInfos { get; set; }
        public List<GetGroupDetailResponseTransactionItem> TransactionInfos { get; set; }
        public GroupExpenseResponse ExpenseGroup { get; set; }
        public List<GetGroupDetailResponseGroupItem> ExpenseCategories { get; set; }
    }
}