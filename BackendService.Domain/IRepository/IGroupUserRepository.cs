using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Domain.Entity;
using BackendService.Domain.Model;

namespace BackendService.Domain.IRepository
{
    public interface IGroupUserRepository : IGenericRepositoryAsync<GroupUsers>
    {
        IEnumerable<GroupUsers> GetByUserId(int userId);
        Task<IEnumerable<UserDto>> GetByGroupId(int groupId);
    }
}