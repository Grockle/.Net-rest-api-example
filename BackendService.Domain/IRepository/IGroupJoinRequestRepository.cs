using System.Linq;
using System.Threading.Tasks;
using BackendService.Domain.Entity;

namespace BackendService.Domain.IRepository
{
    public interface IGroupJoinRequestRepository : IGenericRepositoryAsync<GroupJoinRequest>
    {
        IQueryable<GroupJoinRequest> GetRequestsByShareCode(string shareCode);
        Task<GroupJoinRequest> GetByShareCodeAndUserId(int userId, string shareCode);
    }
}