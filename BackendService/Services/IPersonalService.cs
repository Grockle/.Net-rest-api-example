using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Personal;
using BackendService.Data.DTOs.Personal.Request;
using BackendService.Data.DTOs.Personal.Response;
using BackendService.Data.Entities;

namespace BackendService.Services
{
    public interface IPersonalService
    {
        Task<BaseResponse<bool>> AddPersonalCategory(AddPersonalCategoryRequest personalCategory, string token);
        Task<BaseResponse<GroupedPersonalCategoryDto>> GetPersonalCategories(string token);
        Task<BaseResponse<bool>> UpdatePersonalCategory(UpdatePersonalCategoryRequest personalCategoryRequest, string token);
        Task<BaseResponse<bool>> DeletePersonalCategory(int personalCategoryId, string token);
    }
}