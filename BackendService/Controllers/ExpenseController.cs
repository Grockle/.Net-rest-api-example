using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Expense.Request;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost("AddExpense")]
        public async Task<BaseResponse<bool>> AddExpenseAsync(AddExpensiveRequestDto request)
        {
            return await _expenseService.AddExpenseAsync(request);
        }
    }
}