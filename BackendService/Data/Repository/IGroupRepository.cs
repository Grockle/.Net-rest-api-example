using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BackendService.Data.DTOs.Group.Response;
using Group = BackendService.Data.Entities.Group;

namespace BackendService.Data.Repository
{
    public interface IGroupRepository : IGenericRepositoryAsync<Group>
    {
        Task<Group> GetGroupWithSameNameAsync(string groupName, int userId);
        IEnumerable<GetUserGroupsDto> GetGroupsByUserId(int userId);
        Task<Group> GetGroupByShareCode(string shareCode);
    }
}