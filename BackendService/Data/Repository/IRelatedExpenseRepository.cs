using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IRelatedExpenseRepository : IGenericRepositoryAsync<RelatedExpense>
    {
        Task<bool> AddBulkRelatedExpenses(Expense expense, IQueryable<int> userIds);
    }
}