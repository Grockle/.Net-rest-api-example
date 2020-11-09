using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Transaction.Request;
using BackendService.Data.Entities;
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
        private readonly IEmailService _emailService;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;

        public TransactionService(IUserRepository userRepository, ITransactionRepository transactionRepository, IRelatedTransactionRepository relatedTransactionRepository, IEmailService emailService, IHashService hashService, IDateTimeService dateTimeService)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _relatedTransactionRepository = relatedTransactionRepository;
            _emailService = emailService;
            _hashService = hashService;
            _dateTimeService = dateTimeService;
        }

        public async Task<BaseResponse<bool>> AddTransactionAsync(AddTransactionRequestDto request)
        {
            if (request.RelatedUserIds == null || request.RelatedUserIds.Count == 0)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Related users can not be empty", false);
            }

            var users = _userRepository.GetUsersById(request.RelatedUserIds);

            var transaction = await _transactionRepository.AddAsync(new Transaction
            {
                CreatedBy = request.WhoAdded,
                Amount = request.Amount,
                GroupId = request.GroupId,
                Description = request.Description,
            });

            if (transaction != null)
            {
                var isSucceed = await _relatedTransactionRepository.AddBulkRelatedTransactions(transaction, users.Select(x => x.Id));
                if (isSucceed)
                {
                    return new GeneralMapping<bool>().MapBaseResponse(false, "success", false);
                }
            }
            
            return new GeneralMapping<bool>().MapBaseResponse(true, "Error occuring during adding request", false);
        }
    }
}