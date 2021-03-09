using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Application.Interface.Helper;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;
using BackendService.Domain.Model;
using BackendService.IoC.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BackendService.IoC.Data.Repository
{
    public class GroupRepository : GenericRepositoryAsync<Group>, IGroupRepository
    {
        private readonly DbSet<Group> _groups;
        private readonly DbSet<GroupCategory> _groupCategories;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly ApplicationDbContext _dbContext;
        public GroupRepository(ApplicationDbContext dbContext, IGroupUserRepository groupUserRepository, IDateTimeService dateTimeService) : base(dbContext)
        {
            _dbContext = dbContext;
            _groupUserRepository = groupUserRepository;
            _dateTimeService = dateTimeService;
            _groups = dbContext.Set<Group>();
            _groupCategories = dbContext.Set<GroupCategory>();
        }

        public async Task<Group> GetGroupWithSameNameAsync(string groupName, int userId)
        {
            return await _groups.FirstOrDefaultAsync(x => x.GroupName == groupName && x.CreatedBy == userId);
        }

        public async Task<Group> GetGroupByShareCode(string shareCode)
        {
            return await _groups.FirstOrDefaultAsync(x => x.ShareCode == shareCode);
        }

        public IEnumerable<UserGroupDto> GetUserGroups(int userId)
        {
            var groupUserIds = _groupUserRepository.GetByUserId(userId).Select(x => x.GroupId);
            var groups = _groups.Where(x => groupUserIds.Contains(x.Id)).Select(x => new UserGroupDto
            {
                GroupId = x.Id,
                GroupName = x.GroupName,
                GroupShareCode = x.ShareCode,
                AdminId = x.CreatedBy,
                Description = x.Description,
                Currency = x.MoneyType,
                Budget = x.Budget
            }).ToList();

            return groups;
        }

        public IEnumerable<GroupCategory> GetGroupCategories(int groupId, int type)
        {
            return _groupCategories.Where(x => x.GroupId == groupId && x.Type == type);
        }

        public async Task<GroupCategory> InsertGroupCategory(GroupCategory groupCategory, int currentUserId)
        {
            groupCategory.CreatedBy = currentUserId;
            groupCategory.UpdateBy = currentUserId;
            groupCategory.CreateTime = _dateTimeService.Now;
            await _groupCategories.AddAsync(groupCategory);
            await _dbContext.SaveChangesAsync();
            return groupCategory;
        }
    }
}