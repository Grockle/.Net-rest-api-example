using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class RelatedExpenseRepository : GenericRepositoryAsync<RelatedExpense>, IRelatedExpenseRepository
    {
        private readonly DbSet<RelatedExpense> _relatedExpenses;
        
        public RelatedExpenseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _relatedExpenses = dbContext.Set<RelatedExpense>();
        }

        public async Task<bool> AddBulkRelatedExpenses(Expense expense, IQueryable<int> userIds)
        {
            var relatedExpenses = new List<RelatedExpense>();
            try
            {
                var sharedAmount = expense.Amount / userIds.Count();
                foreach (var userId in userIds)
                {
                    relatedExpenses.Add(new RelatedExpense
                    {
                        CreatedBy = expense.CreatedBy,
                        ExpenseId = expense.Id,
                        GroupId = expense.Id,
                        RelatedAmount = expense.Amount,
                        RelatedUserId = userId
                    });
                }
                await _relatedExpenses.AddRangeAsync(relatedExpenses);
                return true;
            }
            catch (Exception e)
            {
                _relatedExpenses.RemoveRange(relatedExpenses);
                return false;
            }
        }
    }
}