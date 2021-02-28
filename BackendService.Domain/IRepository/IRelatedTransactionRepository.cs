using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Domain.Entity;

namespace BackendService.Domain.IRepository
{
    public interface IRelatedTransactionRepository : IGenericRepositoryAsync<RelatedTransaction>
    {
        Task<bool> InsertAndUpdateBulkExpenses(Transaction transaction, IEnumerable<int> userIds);
    }
}