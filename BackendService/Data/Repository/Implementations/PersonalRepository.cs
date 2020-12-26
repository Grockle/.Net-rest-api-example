using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Context;
using BackendService.Data.DTOs.Personal.Request;
using BackendService.Data.Entities;
using BackendService.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data.Repository.Implementations
{
    public class PersonalRepository : IPersonalRepository
    {
        private readonly DbSet<PersonalAccount> _personalAccounts;
        private readonly DbSet<PersonalCategory> _personalCategories;
        private readonly ApplicationDbContext _dbContext;
        private readonly IDateTimeService _dateTimeService;
        
        public PersonalRepository(ApplicationDbContext dbContext, IDateTimeService dateTimeService)
        {
            _personalAccounts = dbContext.Set<PersonalAccount>();
            _personalCategories = dbContext.Set<PersonalCategory>();
            _dbContext = dbContext;
            _dateTimeService = dateTimeService;
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
        
        public async Task<PersonalCategory> InsertPersonalCategory(PersonalCategory personalCategory, int currentUserId)
        {
            personalCategory.CreatedBy = currentUserId;
            personalCategory.CreateTime = _dateTimeService.Now;
            personalCategory.UpdateBy = currentUserId;
            await _personalCategories.AddAsync(personalCategory);
            await _dbContext.SaveChangesAsync();
            return personalCategory;
        }

        public async Task<PersonalCategory> GetPersonalCategoryByModel(UpdatePersonalCategoryRequest model, int userId)
        {
            return await _personalCategories.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == model.Id && x.Type == model.Type);
        }

        public async Task<PersonalCategory> GetPersonalCategoryById(int id, int userId)
        {
            return await _personalCategories.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
        }
        
        public async Task<PersonalCategory> UpdatePersonalCategory(PersonalCategory personalCategory, int currentUserId)
        {
            personalCategory.UpdateBy = currentUserId;
            personalCategory.UpdateTime = _dateTimeService.Now;
            _personalCategories.Update(personalCategory);
            await _dbContext.SaveChangesAsync();
            return personalCategory;
        }
        
        public async Task DeletePersonalCategory(PersonalCategory personalCategory)
        {
            _personalCategories.Remove(personalCategory);
            await _dbContext.SaveChangesAsync();
        }
    }
}