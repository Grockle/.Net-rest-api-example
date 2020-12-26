using System.Threading.Tasks;
using BackendService.Data.DTOs.Personal;
using BackendService.Data.DTOs.Personal.Request;
using BackendService.Data.DTOs.Personal.Response;
using BackendService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    public class PersonalController : BaseApiController
    {
        private readonly IPersonalService _personalService;

        public PersonalController(IPersonalService personalService)
        {
            _personalService = personalService;
        }
        
        [HttpPost("AddPersonalCategory")]
        public async Task<ActionResult<bool>> AddPersonalCategoryAsync(AddPersonalCategoryRequest request)
        {
            var response = await _personalService.AddPersonalCategory(request, Token);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
        
        [HttpGet("GetPersonalCategories")]
        public async Task<ActionResult<GroupedPersonalCategoryDto>> GetPersonalCategoriesAsync()
        {
            var response = await _personalService.GetPersonalCategories(Token);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
        
        [HttpPut("UpdatePersonalCategory")]
        public async Task<ActionResult<bool>> UpdatePersonalCategoryAsync(UpdatePersonalCategoryRequest request)
        {
            var response = await _personalService.UpdatePersonalCategory(request, Token);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}