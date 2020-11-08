using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Expense.Request;
using BackendService.Data.Entities;
using BackendService.Data.Repository;
using BackendService.Helpers;
using BackendService.Mappings;

namespace BackendService.Services.Implementations
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUserRepository _userRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IRelatedExpenseRepository _relatedExpenseRepository;
        private readonly IEmailService _emailService;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;

        public ExpenseService(IUserRepository userRepository, IExpenseRepository expenseRepository, IRelatedExpenseRepository relatedExpenseRepository, IEmailService emailService, IHashService hashService, IDateTimeService dateTimeService)
        {
            _userRepository = userRepository;
            _expenseRepository = expenseRepository;
            _relatedExpenseRepository = relatedExpenseRepository;
            _emailService = emailService;
            _hashService = hashService;
            _dateTimeService = dateTimeService;
        }

        public async Task<BaseResponse<bool>> AddExpenseAsync(AddExpensiveRequestDto request)
        {
            if (request.RelatedUserIds == null || request.RelatedUserIds.Count == 0)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Related users can not be empty", false);
            }

            var users = _userRepository.GetUsersById(request.RelatedUserIds);

            var expense = await _expenseRepository.AddAsync(new Expense
            {
                CreatedBy = request.WhoAdded,
                Amount = request.Amount,
                GroupId = request.GroupId,
                Description = request.Description,
            });

            if (expense != null)
            {
                var isSucceed = await _relatedExpenseRepository.AddBulkRelatedExpenses(expense, users.Select(x => x.Id));
                if (isSucceed)
                {
                    return new GeneralMapping<bool>().MapBaseResponse(false, "success", false);
                }
            }
            
            return new GeneralMapping<bool>().MapBaseResponse(true, "Error occuring during adding request", false);
        }
    }
}