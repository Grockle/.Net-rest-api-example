using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class UserRepository : GenericRepositoryAsync<User>, IUserRepository
    {
        private readonly DbSet<User> _users;
        
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _users = dbContext.Set<User>();
        }

        public async Task<bool> EmailExist(string email)
        {
            return await _users.AnyAsync(x => x.Email == email);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.FirstOrDefaultAsync(x => x.Email == email);
        }
        
        public async Task<bool> ControlVerification(string email, string code)
        {
            return await _users.AnyAsync(x => x.Email == email && x.VerificationEndTime > DateTime.Now && !x.EmailConfirmed && x.EmailVerificationCode == code);
        }
        
        public async Task<User> GetUserWithoutEmailConfirmedAsync(string email, string password)
        {
            return await _users.FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == password && !x.EmailConfirmed);
        }
        
        public async Task<bool> ControlPasswordResetCodeAsync(string email, string resetCode)
        {
            return await _users.AnyAsync(x => x.Email == email && x.PasswordResetCode == resetCode && x.ResetCodeEndTime > DateTime.Now);
        }
        
        public IQueryable<User> GetUsersById(IQueryable<int> ids)
        {
            return _users.Where(x => ids.Contains(x.Id));
        }
        
        public IQueryable<User> GetUsersById(IEnumerable<int> ids)
        {
            return _users.Where(x => ids.Contains(x.Id));
        }
        
        public async Task<User> GetUserByToken(string token)
        {
            return await _users.FirstOrDefaultAsync(x => x.Token == token);
        }
    }
}