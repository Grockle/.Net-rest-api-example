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
        private readonly IGroupRepository _groupRepository;

        public TransactionService(ITransactionRepository transactionRepository, IRelatedTransactionRepository relatedTransactionRepository, IGroupBudgetBalanceRepository groupBudgetBalanceRepository, IGroupRepository groupRepository)
        {
            _transactionRepository = transactionRepository;
            _relatedTransactionRepository = relatedTransactionRepository;
            _groupBudgetBalanceRepository = groupBudgetBalanceRepository;
            _groupRepository = groupRepository;
        }

        #region Services
        
        public async Task<BaseResponse<bool>> AddExpenseAsync(AddExpenseRequestDto request)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            
            if (request.GroupId == 0 || group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.ZeroGroup;
                return response;
            }
            
            if (request.RelatedUserIds == null || !request.RelatedUserIds.Any())
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyRelatedUsers;
                return response;
            }
            
            if (request.RelatedUserIds.Any(x => x == 0) || request.WhoAdded == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotValidUser;
                return response;
            }

            var transaction = await _transactionRepository.AddAsync(new Transaction
            {
                CreatedBy = request.WhoAdded,
                Amount = request.Amount,
                GroupId = request.GroupId,
                Description = request.Description,
                Type = TransactionType.Expense.ToString(),
                CategoryName = request.Category
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
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            
            if (request.GroupId == 0 || group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.ZeroGroup;
                return response;
            }
            
            if (request.TransactionRelatedUsers == null || !request.TransactionRelatedUsers.Any())
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyRelatedUsers;
                return response;
            }

            if (request.TransactionRelatedUsers.Any(x => x.RelatedUserId == request.WhoAdded))
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotValidUserForTransfer;
                return response;
            }

            if (request.TransactionRelatedUsers.Any(x => x.RelatedUserId == 0) || request.WhoAdded == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotValidUser;
                return response;
            }
            
            if (request.GroupId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.ZeroGroup;
                return response;
            }

            var groupBudgetBalances = _groupBudgetBalanceRepository.GroupBudgetBalancesWithGroupId(request.GroupId).ToArray();

            foreach (var transferredUser in request.TransactionRelatedUsers)
            {
                var transaction = await _transactionRepository.AddAsync(new Transaction
                {
                    CreatedBy = request.WhoAdded,
                    Amount = transferredUser.Amount,
                    GroupId = request.GroupId,
                    Description = request.Description,
                    Type = TransactionType.Transfer.ToString(),
                    CategoryName = "Transfer"
                });

                var relatedTransaction = await _relatedTransactionRepository.AddAsync(new RelatedTransaction
                {
                    TransactionId = transaction.Id,
                    RelatedUserId = transferredUser.RelatedUserId
                });
                    
                
                var transferredBalance = groupBudgetBalances.FirstOrDefault(x => x.UserId == transferredUser.RelatedUserId);

                if (transferredBalance == null)
                {
                    await _groupBudgetBalanceRepository.AddAsync(new GroupBudgetBalance
                    {
                        UserId = transferredUser.RelatedUserId,
                        Balance = -transferredUser.Amount,
                        GroupId = request.GroupId
                    });
                }

                if (transferredBalance != null)
                {
                    transferredBalance.Balance = transferredBalance.Balance - transferredUser.Amount;
                    await _groupBudgetBalanceRepository.UpdateAsync(transferredBalance);
                }
            }

            //ekleyen kişinin hesaplamaları
            var ownerBalance = groupBudgetBalances.FirstOrDefault(x => x.UserId == request.WhoAdded);
            var totalAmount = request.TransactionRelatedUsers.Sum(x => x.Amount);
            
            if (ownerBalance == null)
            {
                await _groupBudgetBalanceRepository.AddAsync(new GroupBudgetBalance
                {
                    UserId = request.WhoAdded,
                    Balance = totalAmount,
                    GroupId = request.GroupId
                });
            }
            else
            {
                ownerBalance.Balance += totalAmount ;
                await _groupBudgetBalanceRepository.UpdateAsync(ownerBalance);
            }
            
            
            response.Data = true;
            return response;
        }

        #endregion
    }
}