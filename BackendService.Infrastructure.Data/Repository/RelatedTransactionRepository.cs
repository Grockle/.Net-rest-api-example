using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;
using BackendService.IoC.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BackendService.IoC.Data.Repository
{
    public class RelatedTransactionRepository : GenericRepositoryAsync<RelatedTransaction>,
        IRelatedTransactionRepository
    {
        private readonly DbSet<RelatedTransaction> _relatedTransactions;
        private readonly IGroupBudgetBalanceRepository _groupBudgetBalanceRepository;

        public RelatedTransactionRepository(ApplicationDbContext dbContext,
            IGroupBudgetBalanceRepository groupBudgetBalanceRepository) : base(dbContext)
        {
            _groupBudgetBalanceRepository = groupBudgetBalanceRepository;
            _relatedTransactions = dbContext.Set<RelatedTransaction>();
        }

        public async Task<bool> InsertAndUpdateBulkExpenses(Transaction transaction, IEnumerable<int> relatedUserIds)
        {
            var relatedTransactionToAdd = new List<RelatedTransaction>();
            var groupBudgetBalanceToUpdate = new List<GroupBudgetBalance>();
            var groupBudgetBalanceToAdd = new List<GroupBudgetBalance>();

            var groupBalance = _groupBudgetBalanceRepository.GroupBudgetBalancesWithGroupId(transaction.GroupId)
                .ToArray();
            var sharedAmount = transaction.Amount / relatedUserIds.Count();
            
            foreach (var relatedUserId in relatedUserIds)
            {
                if (relatedUserId == transaction.CreatedBy)
                {
                    continue;
                }
                
                var personBalance = groupBalance.FirstOrDefault(x => x.UserId == relatedUserId);

                relatedTransactionToAdd.Add(new RelatedTransaction
                {
                    RelatedUserId = relatedUserId,
                    TransactionId = transaction.Id
                });

                if (personBalance != null)
                {
                    personBalance.Balance = personBalance.Balance - sharedAmount;
                    personBalance.UpdateTime = DateTime.Now;
                    personBalance.UpdateBy = transaction.CreatedBy;
                    groupBudgetBalanceToUpdate.Add(personBalance);
                }
                else
                {
                    groupBudgetBalanceToAdd.Add(new GroupBudgetBalance
                    {
                        CreatedBy = transaction.CreatedBy,
                        GroupId = transaction.GroupId,
                        Balance = -sharedAmount,
                        UserId = relatedUserId
                    });
                }
            }
            
            //ekleyen kişinin hesaplaması
            var ownerIsIncluded = relatedUserIds.Any(x => x == transaction.CreatedBy);
            var ownerId = transaction.CreatedBy;
            
            var ownerBalance = groupBalance.FirstOrDefault(x => x.UserId == ownerId);
            if (ownerIsIncluded)
            {
                relatedTransactionToAdd.Add(new RelatedTransaction
                {
                    RelatedUserId = transaction.CreatedBy,
                    TransactionId = transaction.Id
                });
            }

            var netExpenseAmount = ownerIsIncluded ? (transaction.Amount - sharedAmount) : (transaction.Amount);
            
            if (ownerBalance != null)
            {
                ownerBalance.Balance = ownerBalance.Balance + netExpenseAmount;
                ownerBalance.UpdateTime = DateTime.Now;
                ownerBalance.UpdateBy = ownerId;
                groupBudgetBalanceToUpdate.Add(ownerBalance);
            }
            else
            {
                groupBudgetBalanceToAdd.Add(new GroupBudgetBalance
                {
                    CreatedBy = ownerId,
                    GroupId = transaction.GroupId,
                    Balance = netExpenseAmount,
                    UserId = transaction.CreatedBy
                });
            }
            
            if (relatedTransactionToAdd.Count > 0)
            {
                await AddRangeAsync(relatedTransactionToAdd);
            }

            if (groupBudgetBalanceToUpdate.Count > 0)
            {
                await _groupBudgetBalanceRepository.UpdateRangeAsync(groupBudgetBalanceToUpdate);
            }

            if (groupBudgetBalanceToAdd.Count > 0)
            {
                await _groupBudgetBalanceRepository.AddRangeAsync(groupBudgetBalanceToAdd);
            }

            return true;
        }
    }
}