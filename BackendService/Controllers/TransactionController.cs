using System.Threading.Tasks;
using BackendService.Data.DTOs.Transaction.Request;
using BackendService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("AddExpense")]
        public async Task<ActionResult<bool>> AddExpenseAsync(AddExpenseRequestDto request)
        {
            var response = await _transactionService.AddExpenseAsync(request);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
        
        [HttpPost("AddTransfer")]
        public async Task<ActionResult<bool>> AddTransferAsync(AddTransferRequestDto request)
        {
            var response = await _transactionService.AddTransferAsync(request);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}