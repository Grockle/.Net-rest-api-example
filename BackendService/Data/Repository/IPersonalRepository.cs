using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs.Personal.Request;
using BackendService.Data.Entities;

namespace BackendService.Data.Repository
{
    public interface IPersonalRepository
    {
        IEnumerable<PersonalAccount> GetPersonalAccountsByUserId(int userId);
        IEnumerable<PersonalCategory> GetPersonalCategoriesByUserId(int userId);
        Task<PersonalAccount> InsertPersonalAccount(PersonalAccount personalAccount);
        Task<PersonalCategory> InsertPersonalCategory(PersonalCategory personalCategory, int currentUserId);
        Task<PersonalCategory> GetPersonalCategoryByModel(UpdatePersonalCategoryRequest model, int userId);
        Task<PersonalCategory> UpdatePersonalCategory(PersonalCategory personalCategory, int currentUserId);
        Task DeletePersonalCategory(PersonalCategory personalCategory);
        Task<PersonalCategory> GetPersonalCategoryById(int id, int userId);
    }
}