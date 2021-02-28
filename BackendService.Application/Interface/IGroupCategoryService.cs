using System.Threading.Tasks;
using BackendService.Application.Models.Requests.Group;
using BackendService.Application.Models.Responses.General;

namespace BackendService.Application.Interface
{
    public interface IGroupCategoryService
    {
        Task<BaseResponse<bool>> AddAsync(AddGroupCategoryRequest groupCategory, string token);
    }
}
