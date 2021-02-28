using System.Collections.Generic;
using BackendService.Domain.Entity;

namespace BackendService.Domain.IRepository
{
    public interface IGroupBudgetBalanceRepository : IGenericRepositoryAsync<GroupBudgetBalance>
    {
        IEnumerable<GroupBudgetBalance> GroupBudgetBalancesWithGroupId(int groupId);
        void UpdateRange(IEnumerable<GroupBudgetBalance> groupBalances);
    }
}