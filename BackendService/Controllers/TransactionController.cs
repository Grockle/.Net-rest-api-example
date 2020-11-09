using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Transaction.Request;
using BackendService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("AddTransaction")]
        public async Task<BaseResponse<bool>> AddTransactionAsync(AddTransactionRequestDto request)
        {
            return await _transactionService.AddTransactionAsync(request);
        }
    }
}