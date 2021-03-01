using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;
using BackendService.Domain.Model;
using BackendService.IoC.Data.Context;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BackendService.IoC.Data.Repository
{
    public class TransactionRepository : GenericRepositoryAsync<Transaction>, ITransactionRepository
    {
        private readonly DbSet<Transaction> _transactions;
        private readonly IConfiguration _configuration;

        public TransactionRepository(ApplicationDbContext dbContext, IConfiguration configuration) : base(dbContext)
        {
            _configuration = configuration;
            _transactions = dbContext.Set<Transaction>();
        }

        public async Task<IEnumerable<GroupTransactionDto>> GetGroupTransactions(int groupId)
        {
            var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sqlQuery = $@"Select T.""Id"" as TransactionId,T.""CreatedBy"" as AddedBy, T.""Amount"", T.""Description"", T.""Type"",
                                RT.""RelatedUserId"", T.""CreateTime"", T.""CategoryName"" as Category
                                            from public.""Transactions"" as T
                                            Inner Join public.""RelatedTransactions"" as RT
                                            On RT.""TransactionId"" = T.""Id""
                                            WHERE T.""GroupId"" = @groupId";
            return await connection.QueryAsync<GroupTransactionDto>(sqlQuery, new {groupId});
        }
    }
}