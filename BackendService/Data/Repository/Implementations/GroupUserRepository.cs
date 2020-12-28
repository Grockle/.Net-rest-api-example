using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BackendService.Data.Repository.Implementations
{
    public class GroupUserRepository : GenericRepositoryAsync<GroupUsers>, IGroupUserRepository
    {
        private readonly DbSet<GroupUsers> _groupUsers;

        private readonly string ConnectionString =
            "User ID=bahprdjtyanqoq;Password=67d5ff9be3ca216e1df2c5202a6f6621e486d67d878acac3f6a14a1d12827ec6;Server=ec2-52-212-157-46.eu-west-1.compute.amazonaws.com;Port=5432;Database=dfomtj8il10drd;Integrated Security=true;Pooling=true;SSL Mode=Require;TrustServerCertificate=True;";
        public GroupUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _groupUsers = dbContext.Set<GroupUsers>();
        }

        public IEnumerable<GroupUsers> GetByUserId(int userId)
        {
            return _groupUsers.Where(x => x.UserId == userId).ToList();
        }
        
        public async Task<IEnumerable<UserDto>> GetByGroupId(int groupId)
        {
            var connection = new NpgsqlConnection(ConnectionString);
            var sqlQuery = $@"Select groupUsers.*, gp.""Balance"" from (
                                SELECT g.""UserId"" as Id, c.""FirstName"", c.""LastName"", c.""Email""
                                FROM public.""GroupUsers"" g
                                INNER JOIN public.""Users"" c
                                ON c.""Id"" = g.""UserId""
                                WHERE ""GroupId"" = @groupId ) as groupUsers
                                    Left Join public.""GroupBudgetBalances"" as gp
                                    ON gp.""UserId"" = groupUsers.Id
                                ";
            return await connection.QueryAsync<UserDto>(sqlQuery, new {groupId});
        }
        
    }
}