using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IGroupJoinRequestRepository : IGenericRepositoryAsync<GroupJoinRequest>
    {
        IQueryable<GroupJoinRequest> GetRequestsByShareCode(string shareCode);
        Task<GroupJoinRequest> GetByShareCodeAndUserId(int userId, string shareCode);
    }
}