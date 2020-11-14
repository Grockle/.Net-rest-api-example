using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IGroupBudgetBalanceRepository : IGenericRepositoryAsync<GroupBudgetBalance>
    {
        IEnumerable<GroupBudgetBalance> GroupBudgetBalancesWithGroupId(int groupId);
        void UpdateRange(IEnumerable<GroupBudgetBalance> groupBalances);
        Task AddRange(IEnumerable<GroupBudgetBalance> groupBalances);
    }
}