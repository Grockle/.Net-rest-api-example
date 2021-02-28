using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Application.Constants;
using BackendService.Application.Interface;
using BackendService.Application.Models;
using BackendService.Application.Models.Requests.Personal;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.Personal;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;
using BackendService.Domain.Model;
using Mapster;

namespace BackendService.Application.Service
{
    public class PersonalService : IPersonalService
    {
        private readonly IPersonalRepository _personalRepository;
        private readonly IUserRepository _userRepository;

        public PersonalService(IPersonalRepository personalRepository, IUserRepository userRepository)
        {
            _personalRepository = personalRepository;
            _userRepository = userRepository;
        }

        #region Category

        public async Task<BaseResponse<bool>> AddPersonalCategory(AddPersonalCategoryRequest personalCategoryRequest, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (string.IsNullOrEmpty(personalCategoryRequest.CategoryName))
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyCategory;
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
                    Type = personalCategoryRequest.Type,
                    Name = personalCategoryRequest.CategoryName
                }, currentUser.Id);
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

        public async Task<BaseResponse<GroupedPersonalCategoryResponse>> GetPersonalCategories(string token)
        {
            var response = new BaseResponse<GroupedPersonalCategoryResponse>
            {
                HasError = false,
                Data = new GroupedPersonalCategoryResponse
                {
                    ExpenseCategories = new List<GroupedPersonalCategoryResponseItem>(),
                    IncomeCategories = new List<GroupedPersonalCategoryResponseItem>(),
                    AccountCategories = new List<GroupedPersonalCategoryResponseItem>(),
                }
            };

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

        public async Task<BaseResponse<bool>> UpdatePersonalCategory(UpdatePersonalCategoryRequest personalCategoryRequest, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (personalCategoryRequest.Id == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotEditableCategory;
                return response;
            }

            var personalCategory = await _personalRepository.GetPersonalCategoryByModel(personalCategoryRequest.Adapt<PersonalCategoryDto>(), currentUser.Id);

            if (personalCategory == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.CategoryNotExist;
                return response;
            }

            personalCategory.Name = personalCategoryRequest.CategoryName;

            try
            {
                await _personalRepository.UpdatePersonalCategory(personalCategory, currentUser.Id);
                response.Data = true;
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Error = ErrorCodes.Error;
                response.Error.Message = e.Message;
                return response;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeletePersonalCategory(int personalCategoryId, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (personalCategoryId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotEditableCategory;
                return response;
            }

            var personalCategory = await _personalRepository.GetPersonalCategoryById(personalCategoryId, currentUser.Id);

            if (personalCategory == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.CategoryNotExist;
                return response;
            }

            try
            {
                await _personalRepository.DeletePersonalCategory(personalCategory);
                response.Data = true;
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Error = ErrorCodes.Error;
                response.Error.Message = e.Message;
                return response;
            }

            return response;
        }

        #endregion

        #region Account

        public async Task<BaseResponse<bool>> AddPersonalAccount(AddPersonalAccountRequest personalAccountRequest, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (string.IsNullOrEmpty(personalAccountRequest.CategoryName))
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyCategory;
                return response;
            }

            try
            {
                var account = await _personalRepository.InsertPersonalAccount(new PersonalAccount
                {
                    CategoryName = personalAccountRequest.CategoryName,
                    Amount = personalAccountRequest.Amount
                }, currentUser.Id);
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

        public async Task<BaseResponse<bool>> UpdatePersonalAccount(UpdatePersonalAccountRequest personalAccountRequest, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (personalAccountRequest.Id == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotEditableCategory;
                return response;
            }

            if (string.IsNullOrEmpty(personalAccountRequest.CategoryName))
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyCategory;
                return response;
            }

            var personalAccount = await _personalRepository.GetPersonalAccountById(personalAccountRequest.Id, currentUser.Id);

            if (personalAccount == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.CategoryNotExist;
                return response;
            }

            try
            {
                personalAccount.Amount = personalAccountRequest.Amount;
                personalAccount.CategoryName = personalAccountRequest.CategoryName;
                await _personalRepository.UpdatePersonalAccount(personalAccount, currentUser.Id);
                response.Data = true;
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Error = ErrorCodes.Error;
                response.Error.Message = e.Message;
                return response;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeletePersonalAccount(int personalAccountId, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (personalAccountId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotEditableCategory;
                return response;
            }

            var personalAccount = await _personalRepository.GetPersonalAccountById(personalAccountId, currentUser.Id);

            if (personalAccount == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.AccountNotExist;
                return response;
            }

            try
            {
                await _personalRepository.DeletePersonalAccount(personalAccount);
                response.Data = true;
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Error = ErrorCodes.Error;
                response.Error.Message = e.Message;
                return response;
            }

            return response;
        }

        #endregion

        #region PrivateMethods

        private static void SetStaticPersonalCategories(GroupedPersonalCategoryResponse groupedPersonalCategoryDto)
        {
            groupedPersonalCategoryDto.ExpenseCategories.AddRange(ConstantCategory.ExpenseCategory.Select(x => new GroupedPersonalCategoryResponseItem { Id = 0, CategoryName = x, Type = (int)PersonalCategoryType.Expense }));
            groupedPersonalCategoryDto.IncomeCategories.AddRange(ConstantCategory.IncomeCategory.Select(x => new GroupedPersonalCategoryResponseItem { Id = 0, CategoryName = x, Type = (int)PersonalCategoryType.Income }));
            groupedPersonalCategoryDto.AccountCategories.AddRange(ConstantCategory.AccountCategory.Select(x => new GroupedPersonalCategoryResponseItem { Id = 0, CategoryName = x, Type = (int)PersonalCategoryType.Account }));
        }

        private IEnumerable<GroupedPersonalCategoryResponseItem> GetCategoryByType(IEnumerable<PersonalCategory> categories, PersonalCategoryType type)
        {
            return categories.Where(x => x.Type == (int)type).Select(x => new GroupedPersonalCategoryResponseItem
            {
                Id = x.Id,
                CategoryName = x.Name,
                Type = x.Type
            }).ToList();
        }

        #endregion
    }
}