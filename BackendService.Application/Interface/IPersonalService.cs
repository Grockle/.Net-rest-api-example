using System.Threading.Tasks;
using BackendService.Application.Models.Requests.Personal;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.Personal;

namespace BackendService.Application.Interface
{
    public interface IPersonalService
    {
        Task<BaseResponse<bool>> AddPersonalCategory(AddPersonalCategoryRequest personalCategory, string token);
        Task<BaseResponse<GroupedPersonalCategoryResponse>> GetPersonalCategories(string token);
        Task<BaseResponse<bool>> UpdatePersonalCategory(UpdatePersonalCategoryRequest personalCategoryRequest, string token);
        Task<BaseResponse<bool>> DeletePersonalCategory(int personalCategoryId, string token);
        Task<BaseResponse<bool>> AddPersonalAccount(AddPersonalAccountRequest personalAccountRequest, string token);
        Task<BaseResponse<bool>> UpdatePersonalAccount(UpdatePersonalAccountRequest personalAccountRequest, string token);
        Task<BaseResponse<bool>> DeletePersonalAccount(int personalAccountId, string token);
    }
}