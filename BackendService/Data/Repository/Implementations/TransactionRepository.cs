using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.DTOs.Transaction.Response;
using BackendService.Data.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BackendService.Data.Repository.Implementations
{
    public class TransactionRepository : GenericRepositoryAsync<Transaction>, ITransactionRepository
    {
        private readonly DbSet<Transaction> _transactions;
        private readonly string ConnectionString = "User ID=postgres;Password=Bears1234!;Server=34.78.184.211;Port=5432;Database=bears;Integrated Security=true;Pooling=true;";
        
        public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _transactions = dbContext.Set<Transaction>();
        }

        public async Task<IEnumerable<GroupTransactionDto>> GetGroupTransactions(int groupId)
        {
            var connection = new NpgsqlConnection(ConnectionString);
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