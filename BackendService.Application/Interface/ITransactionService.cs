using System.Threading.Tasks;
using BackendService.Application.Models.Requests.Transaction;
using BackendService.Application.Models.Responses.General;

namespace BackendService.Application.Interface
{
    public interface ITransactionService
    {
        Task<BaseResponse<bool>> AddExpenseAsync(AddExpenseRequest request);
        Task<BaseResponse<bool>> AddTransferAsync(AddTransferRequest request);
    }
}