using System.Threading.Tasks;
using BackendService.Application.Interface;
using BackendService.Application.Models.Requests.Group;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.WebApi.Controllers
{
    public class GroupCategoryController : BaseApiController
    {
        private readonly IGroupCategoryService _groupCategoryService;

        public GroupCategoryController(IGroupCategoryService groupCategoryService)
        {
            _groupCategoryService = groupCategoryService;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<bool>> AddAsync(AddGroupCategoryRequest model)
        {
            var response = await _groupCategoryService.AddAsync(model, Token);

            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}
