﻿using System.Threading.Tasks;
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

        [HttpPost("AddExpense")]
        public async Task<BaseResponse<bool>> AddExpenseAsync(AddExpenseRequestDto request)
        {
            return await _transactionService.AddExpenseAsync(request);
        }
        
        [HttpPost("AddTransfer")]
        public async Task<BaseResponse<bool>> AddTransferAsync(AddTransferRequestDto request)
        {
            return await _transactionService.AddTransferAsync(request);
        }
    }
}