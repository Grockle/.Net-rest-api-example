﻿using System.Threading.Tasks;
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
        
        [HttpDelete("DeletePersonalCategory")]
        public async Task<ActionResult<bool>> DeletePersonalCategoryAsync(int personalCategoryId)
        {
            var response = await _personalService.DeletePersonalCategory(personalCategoryId, Token);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
        
        [HttpPost("AddPersonalAccount")]
        public async Task<ActionResult<bool>> AddPersonalAccountAsync(AddPersonalAccountRequest request)
        {
            var response = await _personalService.AddPersonalAccount(request, Token);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
        
        [HttpPut("UpdatePersonalAccount")]
        public async Task<ActionResult<bool>> UpdatePersonalAccountAsync(UpdatePersonalAccountRequest request)
        {
            var response = await _personalService.UpdatePersonalAccount(request, Token);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
        
        [HttpDelete("DeletePersonalAccount")]
        public async Task<ActionResult<bool>> DeletePersonalAccountAsync(int personalAccountId)
        {
            var response = await _personalService.DeletePersonalAccount(personalAccountId, Token);
            
            if (response.HasError)
            {
                return BadRequest(response.Error);
            }

            return Ok(response.Data);
        }
    }
}