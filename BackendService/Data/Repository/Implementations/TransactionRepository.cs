using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class TransactionRepository : GenericRepositoryAsync<Transaction>, ITransactionRepository
    {
        private readonly DbSet<Transaction> _transactions;
        public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _transactions = dbContext.Set<Transaction>();
        }
    }
}