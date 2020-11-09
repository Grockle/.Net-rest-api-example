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
    }
}