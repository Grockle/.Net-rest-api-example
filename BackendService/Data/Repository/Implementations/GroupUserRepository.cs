using System.Collections.Generic;
using System.Linq;
using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class GroupUserRepository : GenericRepositoryAsync<GroupUsers>, IGroupUserRepository
    {
        private readonly DbSet<GroupUsers> _groupUsers;

        public GroupUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _groupUsers = dbContext.Set<GroupUsers>();
        }

        public IEnumerable<GroupUsers> GetByUserId(int userId)
        {
            return _groupUsers.Where(x => x.UserId == userId).ToList();
        }
        
        public IQueryable<GroupUsers> GetByGroupId(int groupId)
        {
            return _groupUsers.Where(x => x.GroupId == groupId);
        }
        
    }
}