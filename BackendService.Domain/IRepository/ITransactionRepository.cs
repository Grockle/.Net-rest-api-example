using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Domain.Entity;
using BackendService.Domain.Model;

namespace BackendService.Domain.IRepository
{
    public interface ITransactionRepository : IGenericRepositoryAsync<Transaction>
    {
        Task<IEnumerable<GroupTransactionDto>> GetGroupTransactions(int groupId);
    }
}