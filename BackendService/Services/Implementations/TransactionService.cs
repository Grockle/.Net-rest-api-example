using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Transaction.Request;
using BackendService.Data.Entities;
using BackendService.Data.Enums;
using BackendService.Data.Repository;

namespace BackendService.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRelatedTransactionRepository _relatedTransactionRepository;
        private readonly IGroupBudgetBalanceRepository _groupBudgetBalanceRepository;

        public TransactionService(ITransactionRepository transactionRepository, IRelatedTransactionRepository relatedTransactionRepository, IGroupBudgetBalanceRepository groupBudgetBalanceRepository)
        {
            _transactionRepository = transactionRepository;
            _relatedTransactionRepository = relatedTransactionRepository;
            _groupBudgetBalanceRepository = groupBudgetBalanceRepository;
        }

        #region Services
        
        public async Task<BaseResponse<bool>> AddExpenseAsync(AddExpenseRequestDto request)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            
            if (request.RelatedUserIds == null || !request.RelatedUserIds.Any())
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyRelatedUsers;
                return response;
            }

            if (request.TransactionType != TransactionType.Expense.ToString())
            {
                response.HasError = true;
                response.Error = ErrorCodes.TransactionTypeError;
                return response;
            }
            
            var transaction = await _transactionRepository.AddAsync(new Transaction
            {
                CreatedBy = request.WhoAdded,
                Amount = request.Amount,
                GroupId = request.GroupId,
                Description = request.Description,
                Type = TransactionType.Expense.ToString()
            });

            if (transaction != null)
            {
                var isSucceed = await _relatedTransactionRepository.InsertAndUpdateBulkExpenses(transaction, request.RelatedUserIds);
                
                if (isSucceed)
                {
                    response.Data = true;
                    return response;
                }
            }

            response.HasError = true;
            response.Error = ErrorCodes.CommonProcessError;
            return response;
        }
        public async Task<BaseResponse<bool>> AddTransferAsync(AddTransferRequestDto request)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            
            if (request.RelatedUsers == null || !request.RelatedUsers.Any())
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyRelatedUsers;
                return response;
            }

            if (request.TransactionType != TransactionType.Transfer.ToString())
            {
                response.HasError = true;
                response.Error = ErrorCodes.TransactionTypeError;
                return response;
            }

            var groupBudgetBalances = _groupBudgetBalanceRepository.GroupBudgetBalancesWithGroupId(request.GroupId).ToArray();

            foreach (var transferredUser in request.RelatedUsers)
            {
                if (transferredUser.RelatedUserId != request.WhoAdded)
                {
                    var transaction = await _transactionRepository.AddAsync(new Transaction
                    {
                        CreatedBy = request.WhoAdded,
                        Amount = transferredUser.Amount,
                        GroupId = request.GroupId,
                        Description = request.Description,
                        Type = TransactionType.Transfer.ToString()
                    });

                    var relatedTransaction = await _relatedTransactionRepository.AddAsync(new RelatedTransaction
                    {
                        TransactionId = transaction.Id,
                        RelatedUserId = transferredUser.RelatedUserId
                    });
                    
                    var ownerBalance = groupBudgetBalances.FirstOrDefault(x => x.UserId == request.WhoAdded);
                    var transferredBalance = groupBudgetBalances.FirstOrDefault(x => x.UserId == transferredUser.RelatedUserId);

                    if (ownerBalance == null)
                    {
                        await _groupBudgetBalanceRepository.AddAsync(new GroupBudgetBalance
                        {
                            UserId = request.WhoAdded,
                            Balance = transferredUser.Amount,
                            GroupId = request.GroupId
                        });
                    }

                    if (transferredBalance == null)
                    {
                        await _groupBudgetBalanceRepository.AddAsync(new GroupBudgetBalance
                        {
                            UserId = transferredUser.RelatedUserId,
                            Balance = -transferredUser.Amount,
                            GroupId = request.GroupId
                        });
                    }

                    if (ownerBalance != null)
                    {
                        ownerBalance.Balance = ownerBalance.Balance + transferredUser.Amount;
                        await _groupBudgetBalanceRepository.UpdateAsync(ownerBalance);
                    }
                    
                    if (transferredBalance != null)
                    {
                        transferredBalance.Balance = transferredBalance.Balance - transferredUser.Amount;
                        await _groupBudgetBalanceRepository.UpdateAsync(ownerBalance);
                    }
                }
            }

            response.Data = true;
            return response;
        }
        
        #endregion
    }
}