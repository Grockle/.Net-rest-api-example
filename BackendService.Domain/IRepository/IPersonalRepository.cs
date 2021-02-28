using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Domain.Entity;
using BackendService.Domain.Model;

namespace BackendService.Domain.IRepository
{
    public interface IPersonalRepository
    {
        IEnumerable<PersonalCategory> GetPersonalCategoriesByUserId(int userId);
        Task<PersonalAccount> InsertPersonalAccount(PersonalAccount personalAccount, int currentUserId);
        Task<PersonalCategory> InsertPersonalCategory(PersonalCategory personalCategory, int currentUserId);
        Task<PersonalCategory> GetPersonalCategoryByModel(PersonalCategoryDto model, int userId);
        Task<PersonalCategory> UpdatePersonalCategory(PersonalCategory personalCategory, int currentUserId);
        Task DeletePersonalCategory(PersonalCategory personalCategory);
        Task<PersonalCategory> GetPersonalCategoryById(int id, int userId);
        Task<PersonalAccount> GetPersonalAccountById(int id, int userId);
        Task<PersonalAccount> UpdatePersonalAccount(PersonalAccount personalAccount, int currentUserId);
        Task DeletePersonalAccount(PersonalAccount personalAccount);
    }
}