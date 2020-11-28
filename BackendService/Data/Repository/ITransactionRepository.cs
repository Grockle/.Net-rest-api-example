using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Data.DTOs.Transaction.Response;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface ITransactionRepository : IGenericRepositoryAsync<Transaction>
    {
        Task<IEnumerable<GroupTransactionDto>> GetGroupTransactions(int groupId);
    }
}