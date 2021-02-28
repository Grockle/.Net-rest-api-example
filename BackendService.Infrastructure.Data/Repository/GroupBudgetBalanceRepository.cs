using System.Collections.Generic;
using System.Linq;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;
using BackendService.IoC.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BackendService.IoC.Data.Repository
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