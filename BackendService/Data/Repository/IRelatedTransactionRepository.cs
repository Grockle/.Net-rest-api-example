using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs.Transaction.Request;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IRelatedTransactionRepository : IGenericRepositoryAsync<RelatedTransaction>
    {
        Task<bool> InsertAndUpdateBulkExpenses(Transaction transaction, IEnumerable<int> userIds);
    }
}