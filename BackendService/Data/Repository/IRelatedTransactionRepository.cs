using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IRelatedTransactionRepository : IGenericRepositoryAsync<RelatedTransaction>
    {
        Task<bool> AddBulkRelatedTransactions(Transaction transaction, IQueryable<int> userIds);
    }
}