using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class GroupBudgetBalanceRepository : GenericRepositoryAsync<GroupBudgetBalance>, IGroupBudgetBalanceRepository
    {
        private readonly DbSet<GroupBudgetBalance> _groupBudgetBalances;
        
        public GroupBudgetBalanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _groupBudgetBalances = dbContext.Set<GroupBudgetBalance>();
        }

        public IEnumerable<GroupBudgetBalance> GroupBudgetBalancesWithGroupId(int groupId)
        {
            return _groupBudgetBalances.Where(x => x.GroupId == groupId);
        }

        public void UpdateRange(IEnumerable<GroupBudgetBalance> groupBalances)
        {
            _groupBudgetBalances.UpdateRange(groupBalances);
        }
        
    }
}