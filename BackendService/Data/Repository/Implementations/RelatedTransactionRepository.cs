using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.DTOs.Transaction.Request;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
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

        public async Task<bool> InsertAndUpdateBulkExpenses(Transaction transaction, List<int> relatedUserIds)
        {
            var relatedTransactionToAdd = new List<RelatedTransaction>();
            var groupBudgetBalanceToUpdate = new List<GroupBudgetBalance>();
            var groupBudgetBalanceToAdd = new List<GroupBudgetBalance>();

            var groupBalance = _groupBudgetBalanceRepository.GroupBudgetBalancesWithGroupId(transaction.GroupId)
                .ToArray();
            var sharedAmount = transaction.Amount / relatedUserIds.Count();
            
            foreach (var relatedUserId in relatedUserIds)
            {
                var personBalance =
                    groupBalance.FirstOrDefault(x => x.UserId == relatedUserId);

                relatedTransactionToAdd.Add(new RelatedTransaction
                {
                    RelatedUserId = relatedUserId,
                    TransactionId = transaction.Id
                });

                if (personBalance != null)
                {
                    var amount = (transaction.CreatedBy == relatedUserId)
                        ? (personBalance.Balance +
                           (transaction.Amount - sharedAmount))
                        : (personBalance.Balance - sharedAmount);

                    personBalance.Balance = amount;
                    personBalance.UpdateTime = DateTime.Now;
                    personBalance.UpdateBy = transaction.CreatedBy;
                    groupBudgetBalanceToUpdate.Add(personBalance);
                }
                else
                {
                    var amount = (transaction.CreatedBy == relatedUserId)
                        ? (transaction.Amount - sharedAmount)
                        : (-sharedAmount);

                    groupBudgetBalanceToAdd.Add(new GroupBudgetBalance
                    {
                        CreatedBy = transaction.CreatedBy,
                        GroupId = transaction.Id,
                        Balance = amount,
                        UserId = relatedUserId
                    });
                }
            }

            if (relatedTransactionToAdd.Count > 0)
            {
                await _relatedTransactions.AddRangeAsync(relatedTransactionToAdd);
            }

            if (groupBudgetBalanceToUpdate.Count > 0)
            {
                _groupBudgetBalanceRepository.UpdateRange(groupBudgetBalanceToUpdate);
            }

            if (groupBudgetBalanceToAdd.Count > 0)
            {
                _groupBudgetBalanceRepository.UpdateRange(groupBudgetBalanceToUpdate);
            }

            return true;
        }
    }
}