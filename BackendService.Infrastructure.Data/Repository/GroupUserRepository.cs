using System.Collections.Generic;
using System.Linq;
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
    public class GroupUserRepository : GenericRepositoryAsync<GroupUsers>, IGroupUserRepository
    {
        private readonly DbSet<GroupUsers> _groupUsers;
        private readonly IConfiguration _configuration;
        public GroupUserRepository(ApplicationDbContext dbContext, IConfiguration configuration) : base(dbContext)
        {
            _configuration = configuration;
            _groupUsers = dbContext.Set<GroupUsers>();
        }

        public IEnumerable<GroupUsers> GetByUserId(int userId)
        {
            return _groupUsers.Where(x => x.UserId == userId).ToList();
        }

        public async Task<IEnumerable<UserDto>> GetByGroupId(int groupId)
        {
            var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sqlQuery = $@"Select groupUsers.*, gp.""Balance"" from (
                                SELECT g.""UserId"" as Id, c.""FirstName"", c.""LastName"", c.""Email""
                                FROM public.""GroupUsers"" g
                                INNER JOIN public.""Users"" c
                                ON c.""Id"" = g.""UserId""
                                WHERE ""GroupId"" = @groupId ) as groupUsers
                                    Left Join public.""GroupBudgetBalances"" as gp
                                    ON gp.""UserId"" = groupUsers.Id
                                ";
            return await connection.QueryAsync<UserDto>(sqlQuery, new { groupId });
        }

    }
}