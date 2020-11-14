using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Transaction.Request;

namespace BackendService.Services
{
    public interface ITransactionService
    {
        Task<BaseResponse<bool>> AddExpenseAsync(AddExpenseRequestDto request);
        Task<BaseResponse<bool>> AddTransferAsync(AddTransferRequestDto request);
    }
}