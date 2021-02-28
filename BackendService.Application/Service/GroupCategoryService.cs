using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendService.Application.Constants;
using BackendService.Application.Interface;
using BackendService.Application.Models.Requests.Group;
using BackendService.Application.Models.Responses.General;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;

namespace BackendService.Application.Service
{
    public class GroupCategoryService : IGroupCategoryService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;

        public GroupCategoryService(IGroupRepository groupRepository, IUserRepository userRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<bool>> AddAsync(AddGroupCategoryRequest groupCategory, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var user = await _userRepository.GetUserByToken(token);
            var group = await _groupRepository.GetGroupByShareCode(groupCategory.GroupShareCode);

            if (user == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            if (group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupIsNotExist;
                return response;
            }

            if (string.IsNullOrEmpty(groupCategory.Name))
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyCategory;
                return response;
            }

            if (!Enum.IsDefined(typeof(GroupCategoryType), groupCategory.Type))
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotValidCategoryType;
                return response;
            }

            var categories = _groupRepository.GetGroupCategories(group.Id, groupCategory.Type);
            var isCategoryExist = categories.Any(x => x.Name == groupCategory.Name) ||
                                  ConstantCategory.GroupExpenseCategory.Any(x => x == groupCategory.Name);
            if (isCategoryExist)
            {
                response.HasError = true;
                response.Error = ErrorCodes.CategoryExist;
                return response;
            }

            try
            {
                await _groupRepository.InsertGroupCategory(new GroupCategory
                {
                    Type = groupCategory.Type,
                    Name = groupCategory.Name,
                    GroupId = group.Id
                }, user.Id);
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
    }
}
