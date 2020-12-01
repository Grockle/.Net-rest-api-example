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

        private readonly string ConnectionString = "User ID=postgres;Password=Bears1234!;Server=34.78.184.211;Port=5432;Database=bears;Integrated Security=true;Pooling=true;";
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