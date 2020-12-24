using System.Collections.Generic;
using BackendService.Data.DTOs.GroupBudgetBalance;
using BackendService.Data.DTOs.Transaction.Response;
using BackendService.Data.DTOs.User.Response;

namespace BackendService.Data.DTOs.Group.Response
{
    public class GetGroupDetailDto
    {
        public int GroupId { get; set; }
        public int AdminId { get; set; }
        public string ShareCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public List<UserDto> UserInfos { get; set; }
        public List<TransactionDto> TransactionInfos { get; set; }
    }
}