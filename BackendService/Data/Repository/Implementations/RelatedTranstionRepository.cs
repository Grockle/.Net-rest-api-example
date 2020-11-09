using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class RelatedTransactionRepository : GenericRepositoryAsync<RelatedTransaction>, IRelatedTransactionRepository
    {
        private readonly DbSet<RelatedTransaction> _relatedTransactions;
        
        public RelatedTransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _relatedTransactions = dbContext.Set<RelatedTransaction>();
        }

        public async Task<bool> AddBulkRelatedTransactions(Transaction transaction, IQueryable<int> userIds)
        {
            var relatedTransactions = new List<RelatedTransaction>();
            try
            {
                var sharedAmount = transaction.Amount / userIds.Count();
                foreach (var userId in userIds)
                {
                    if (transaction.CreatedBy == userId)
                    {
                        relatedTransactions.Add(new RelatedTransaction
                        {
                            CreatedBy = transaction.CreatedBy,
                            GroupId = transaction.Id,
                            RelatedAmount = transaction.Amount - sharedAmount,
                            RelatedUserId = userId
                        });
                    }
                    else
                    {
                        relatedTransactions.Add(new RelatedTransaction
                        {
                            CreatedBy = transaction.CreatedBy,
                            GroupId = transaction.Id,
                            RelatedAmount = -sharedAmount,
                            RelatedUserId = userId
                        });
                    }
                    
                }
                await _relatedTransactions.AddRangeAsync(relatedTransactions);
                return true;
            }
            catch (Exception e)
            {
                _relatedTransactions.RemoveRange(relatedTransactions);
                return false;
            }
        }
    }
}