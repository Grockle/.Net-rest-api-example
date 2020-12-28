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

        private readonly string ConnectionString =
            "User ID=bahprdjtyanqoq;Password=67d5ff9be3ca216e1df2c5202a6f6621e486d67d878acac3f6a14a1d12827ec6;Server=ec2-52-212-157-46.eu-west-1.compute.amazonaws.com;Port=5432;Database=dfomtj8il10drd;Integrated Security=true;Pooling=true;SSL Mode=Require;TrustServerCertificate=True;";


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