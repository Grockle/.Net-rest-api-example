using System.Collections.Generic;
using System.Linq;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IGroupUserRepository : IGenericRepositoryAsync<GroupUsers>
    {
        List<GroupUsers> GetByUserId(int userId);
        IQueryable<GroupUsers> GetByGroupId(int groupId);
    }
}