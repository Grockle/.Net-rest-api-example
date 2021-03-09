using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Domain.Entity;
using BackendService.Domain.Model;

namespace BackendService.Domain.IRepository
{
    public interface IGroupRepository : IGenericRepositoryAsync<Entity.Group>
    {
        Task<Group> GetGroupWithSameNameAsync(string groupName, int userId);
        IEnumerable<UserGroupDto> GetUserGroups(int userId);
        Task<Group> GetGroupByShareCode(string shareCode);
        IEnumerable<GroupCategory> GetGroupCategories(int groupId, int type);
        Task<GroupCategory> InsertGroupCategory(GroupCategory groupCategory, int currentUserId);
    }
}