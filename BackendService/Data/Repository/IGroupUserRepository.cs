using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IGroupUserRepository : IGenericRepositoryAsync<GroupUsers>
    {
        IEnumerable<GroupUsers> GetByUserId(int userId);
        Task<IEnumerable<UserDto>> GetByGroupId(int groupId);
    }
}