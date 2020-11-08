using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Expense.Request;

namespace BackendService.Services
{
    public interface IExpenseService
    {
        Task<BaseResponse<bool>> AddExpenseAsync(AddExpensiveRequestDto request);
    }
}