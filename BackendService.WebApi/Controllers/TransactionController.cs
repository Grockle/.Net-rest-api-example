using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Models.Requests.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("Expense")]
        public async Task<ActionResult<bool>> AddExpenseAsync(AddExpenseRequest request)
        {
            var response = await _transactionService.AddExpenseAsync(request);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
        
        [HttpPost("Transfer")]
        public async Task<ActionResult<bool>> AddTransferAsync(AddTransferRequest request)
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