using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Transaction.Request;
using BackendService.Data.Entities;
using BackendService.Data.Enums;
using BackendService.Data.Repository;
using BackendService.Helpers;
using BackendService.Mappings;

namespace BackendService.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRelatedTransactionRepository _relatedTransactionRepository;
        private readonly IGroupBudgetBalanceRepository _groupBudgetBalanceRepository;
        private readonly IEmailService _emailService;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;

        public TransactionService(IUserRepository userRepository, ITransactionRepository transactionRepository, IRelatedTransactionRepository relatedTransactionRepository, IEmailService emailService, IHashService hashService, IDateTimeService dateTimeService, IGroupBudgetBalanceRepository groupBudgetBalanceRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _relatedTransactionRepository = relatedTransactionRepository;
            _emailService = emailService;
            _hashService = hashService;
            _dateTimeService = dateTimeService;
            _groupBudgetBalanceRepository = groupBudgetBalanceRepository;
        }

        public async Task<BaseResponse<bool>> AddExpenseAsync(AddExpenseRequestDto request)
        {
            if (request.RelatedUserIds == null || request.RelatedUserIds.Count == 0)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Related users can not be empty", false);
            }

            if (request.TransactionType != TransactionType.Expense.ToString())
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Invalid Transaction Type", false);
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
                    return new GeneralMapping<bool>().MapBaseResponse(false, "success", false);
                }
            }
            
            return new GeneralMapping<bool>().MapBaseResponse(true, "Error occuring during adding request", false);
        }
        
        public async Task<BaseResponse<bool>> AddTransferAsync(AddTransferRequestDto request)
        {
            if (request.RelatedUsers == null || request.RelatedUsers.Count == 0)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Related users can not be empty", false);
            }

            if (request.TransactionType != TransactionType.Transfer.ToString())
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Invalid Transaction Type", false);
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
            return new GeneralMapping<bool>().MapBaseResponse(false, "Success Transfer", true);
        }
    }
}