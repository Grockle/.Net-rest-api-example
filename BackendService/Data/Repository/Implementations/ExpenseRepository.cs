using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class ExpenseRepository : GenericRepositoryAsync<Expense>, IExpenseRepository
    {
        private readonly DbSet<Expense> _expenses;
        public ExpenseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _expenses = dbContext.Set<Expense>();
        }
    }
}