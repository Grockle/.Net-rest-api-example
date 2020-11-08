using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class GroupJoinRequestRepository : GenericRepositoryAsync<GroupJoinRequest>, IGroupJoinRequestRepository
    {
        private readonly DbSet<GroupJoinRequest> _groupJoinRequests;

        public GroupJoinRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _groupJoinRequests = dbContext.Set<GroupJoinRequest>();
        }

        public IQueryable<GroupJoinRequest> GetRequestsByShareCode(string shareCode)
        {
            return _groupJoinRequests.Where(x => x.GroupShareCode == shareCode && x.IsActive);
        }

        public async Task<GroupJoinRequest> GetByShareCodeAndUserId(int userId, string shareCode)
        {
            return await _groupJoinRequests.FirstOrDefaultAsync(x =>
                x.FromUserId == userId && x.IsActive && x.GroupShareCode == shareCode);
        }
    }
}