using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Personal;
using BackendService.Data.DTOs.Personal.Request;
using BackendService.Data.DTOs.Personal.Response;
using BackendService.Data.Entities;
using BackendService.Data.Enums;
using BackendService.Data.Repository;
using BackendService.Helpers;

namespace BackendService.Services.Implementations
{
    public class PersonalService : IPersonalService
    {
        private readonly IPersonalRepository _personalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDateTimeService _dateTimeService;

        public PersonalService(IPersonalRepository personalRepository, IUserRepository userRepository, IDateTimeService dateTimeService)
        {
            _personalRepository = personalRepository;
            _userRepository = userRepository;
            _dateTimeService = dateTimeService;
        }

        public async Task<BaseResponse<bool>> AddPersonalCategory(AddPersonalCategoryRequest personalCategoryRequest,string token)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (!Enum.IsDefined(typeof(PersonalCategoryType), personalCategoryRequest.Type))
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotValidCategoryType;
                return response;
            }

            try
            {
                await _personalRepository.InsertPersonalCategory(new PersonalCategory
                {
                    CreatedBy = currentUser.Id,
                    CreateTime = _dateTimeService.Now,
                    UpdateBy = currentUser.Id,
                    UserId = currentUser.Id,
                    Type = personalCategoryRequest.Type,
                    Name = personalCategoryRequest.CategoryName
                });
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Error = ErrorCodes.Error;
                response.Error.Message = e.Message;
                return response;
            }

            response.Data = true;
            return response;
        }

        public async Task<BaseResponse<GroupedPersonalCategoryDto>> GetPersonalCategories(string token)
        {
            var response = new BaseResponse<GroupedPersonalCategoryDto>
                {HasError = false, Data = new GroupedPersonalCategoryDto
                {
                    ExpenseCategories = new List<PersonalCategoryDto>(),
                    IncomeCategories = new List<PersonalCategoryDto>(),
                    AccountCategories = new List<PersonalCategoryDto>(),
                }};

            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            try
            {
                var personalCategories = _personalRepository.GetPersonalCategoriesByUserId(currentUser.Id).ToList();
                SetStaticPersonalCategories(response.Data);
                
                if (personalCategories.Any())
                {
                    response.Data.ExpenseCategories.AddRange(GetCategoryByType(personalCategories, PersonalCategoryType.Expense));
                    response.Data.IncomeCategories.AddRange(GetCategoryByType(personalCategories, PersonalCategoryType.Income));
                    response.Data.AccountCategories.AddRange(GetCategoryByType(personalCategories, PersonalCategoryType.Account));
                }

                response.Data.ExpenseCategories = response.Data.ExpenseCategories.OrderBy(x => x.CategoryName).ToList();
                response.Data.IncomeCategories = response.Data.IncomeCategories.OrderBy(x => x.CategoryName).ToList();
                response.Data.AccountCategories = response.Data.AccountCategories.OrderBy(x => x.CategoryName).ToList();

            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Error = ErrorCodes.Error;
                response.Error.Message = e.Message;
                response.Data = null;
                return response;
            }


            return response;
        }

        private void SetStaticPersonalCategories(GroupedPersonalCategoryDto groupedPersonalCategoryDto)
        {
            groupedPersonalCategoryDto.ExpenseCategories.AddRange(ConstantCategory.ExpenseCategory.Select(x => new PersonalCategoryDto{Id = 0, CategoryName = x,Type = (int)PersonalCategoryType.Expense}));
            groupedPersonalCategoryDto.IncomeCategories.AddRange(ConstantCategory.IncomeCategory.Select(x => new PersonalCategoryDto{Id = 0, CategoryName = x,Type = (int)PersonalCategoryType.Income}));
            groupedPersonalCategoryDto.AccountCategories.AddRange(ConstantCategory.AccountCategory.Select(x => new PersonalCategoryDto{Id = 0, CategoryName = x,Type = (int)PersonalCategoryType.Account}));
        }

        private IEnumerable<PersonalCategoryDto> GetCategoryByType(IEnumerable<PersonalCategory> categories,
            PersonalCategoryType type)
        {
            return categories.Where(x => x.Type == (int) type).Select(x => new PersonalCategoryDto
            {
                Id = x.Id,
                CategoryName = x.Name,
                Type = x.Type
            }).ToList();
        }
    }
}