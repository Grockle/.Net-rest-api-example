using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class PersonalRepository : IPersonalRepository
    {
        private readonly DbSet<PersonalAccount> _personalAccounts;
        private readonly DbSet<PersonalCategory> _personalCategories;
        private readonly ApplicationDbContext _dbContext;
        
        public PersonalRepository(ApplicationDbContext dbContext)
        {
            _personalAccounts = dbContext.Set<PersonalAccount>();
            _personalCategories = dbContext.Set<PersonalCategory>();
            _dbContext = dbContext;
        }
        
        public IEnumerable<PersonalAccount> GetPersonalAccountsByUserId(int userId)
        {
            return _personalAccounts.Where(x => x.UserId == userId);;
        }
        
        public IEnumerable<PersonalCategory> GetPersonalCategoriesByUserId(int userId)
        {
            return _personalCategories.Where(x => x.UserId == userId);
        }

        public async Task<PersonalAccount> InsertPersonalAccount(PersonalAccount personalAccount)
        {
            await _personalAccounts.AddAsync(personalAccount);
            await _dbContext.SaveChangesAsync();
            return personalAccount;
        }
        
        public async Task<PersonalCategory> InsertPersonalCategory(PersonalCategory personalCategory)
        {
            await _personalCategories.AddAsync(personalCategory);
            await _dbContext.SaveChangesAsync();
            return personalCategory;
        }
        
    }
}