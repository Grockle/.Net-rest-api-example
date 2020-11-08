using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.DTOs.Group.Response;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class GroupRepository : GenericRepositoryAsync<Group>, IGroupRepository
    {
        private readonly DbSet<Group> _groups;
        private readonly IGroupUserRepository _groupUserRepository;

        public GroupRepository(ApplicationDbContext dbContext, IGroupUserRepository groupUserRepository) : base(dbContext)
        {
            _groupUserRepository = groupUserRepository;
            _groups = dbContext.Set<Group>();
        }

        public async Task<Group> GetGroupWithSameNameAsync(string groupName, int userId)
        {
            return await _groups.FirstOrDefaultAsync(x => x.GroupName == groupName && x.CreatedBy == userId);
        }
        
        public async Task<Group> GetGroupByShareCode(string shareCode)
        {
            return await _groups.FirstOrDefaultAsync(x => x.ShareCode == shareCode);
        }

        public List<GetUserGroupsDto> GetGroupsByUserId(int userId)
        {
            var groupUserIds = _groupUserRepository.GetByUserId(userId).Select(x => x.GroupId);
            var groups = _groups.Where(x => groupUserIds.Contains(x.Id)).Select(x => new GetUserGroupsDto
            {
                GroupId = x.Id,
                GroupName = x.GroupName,
                GroupShareCode = x.ShareCode,
                AdminId = x.CreatedBy,
                Description = x.Description
                
            }).ToList();
            
            return groups;
        }
    }
}