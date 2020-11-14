using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IUserRepository : IGenericRepositoryAsync<User>
    {
        Task<bool> EmailExist(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> ControlVerification(string email, string code);
        Task<User> GetUserWithoutEmailConfirmedAsync(string email, string password);
        Task<bool> ControlPasswordResetCodeAsync(string email, string resetCode);
        IQueryable<User> GetUsersById(IQueryable<int> ids);
        IQueryable<User> GetUsersById(IEnumerable<int> ids);
        Task<User> GetUserByToken(string token);
    }
}