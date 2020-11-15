using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Group.Request;
using BackendService.Data.DTOs.Group.Response;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;
using BackendService.Data.Enums;
using BackendService.Data.Repository;
using BackendService.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IGroupJoinRequestRepository _groupJoinRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;

        public GroupService(IHashService hashService, IGroupRepository groupRepository, IGroupUserRepository groupUserRepository, IGroupJoinRequestRepository groupJoinRequestRepository, IUserRepository userRepository)
        {
            _hashService = hashService;
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
            _groupJoinRequestRepository = groupJoinRequestRepository;
            _userRepository = userRepository;
        }

        #region Services

        public async Task<BaseResponse<bool>> AddGroupAsync(AddGroupRequest model)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            
            if (model.UserId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserIdRequired;
                return response;
            }
            
            var existGroup = await _groupRepository.GetGroupWithSameNameAsync(model.GroupName, model.UserId);
            
            if (existGroup != null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupNameExist;
                return response;
            }

            var group = await _groupRepository.AddAsync(new Group
            {
                CreatedBy = model.UserId,
                ShareCode = _hashService.EncryptString(model.UserId + _hashService.GenerateCode()),
                GroupName = model.GroupName,
                Description = model.Description,
                MoneyType = model.MoneyShortCut
            });

            if (group != null)
            {
                var groupUser = await _groupUserRepository.AddAsync(new GroupUsers
                {
                    UserId = model.UserId,
                    GroupId = group.Id,
                });
                response.Data = true;
                return response;
            }
            response.HasError = true;
            response.Error = ErrorCodes.AddGroupFailed;
            return response;
        }
        public async Task<BaseResponse<IEnumerable<GetUserGroupsDto>>> GetUserGroupsAsync(int userId)
        {
            var response = new BaseResponse<IEnumerable<GetUserGroupsDto>> {HasError = false, Data = null};
            if (userId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserIdRequired;
                return response;
            }

            var groups = _groupRepository.GetGroupsByUserId(userId);
            
            response.Data = groups;
            return response;
        }
        public async Task<BaseResponse<IEnumerable<GetGroupJoinRequestsDto>>> GetGroupJoinRequests(string shareCode)
        {
            var response = new BaseResponse<IEnumerable<GetGroupJoinRequestsDto>> {HasError = false, Data = null};
            if (string.IsNullOrEmpty(shareCode))
            {
                response.HasError = true;
                response.Error = ErrorCodes.ShareCodeRequired;
                return response;
            }
            
            var groupJoinRequests = _groupJoinRequestRepository.GetRequestsByShareCode(shareCode);
            
            if (groupJoinRequests == null)
            {
                response.Data = null;
                response.HasError = false;
                return response;
            }

            var users = _userRepository.GetUsersById(groupJoinRequests.Select(x => x.FromUserId)).Select(x =>
                new GetGroupJoinRequestsDto
                {
                    UserId = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    RequestId = groupJoinRequests.FirstOrDefault(y => y.FromUserId == x.Id).Id
                }).ToList();

            response.Data = users;
            return response;
        }
        public async Task<BaseResponse<bool>>SendGroupJoinRequest(int userId, string shareCode)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            if (userId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserIdRequired;
                return response;
            }

            var group = await _groupRepository.GetGroupByShareCode(shareCode);

            if (group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupIsNotExist;
                return response;
            }

            var existRequest = await _groupJoinRequestRepository.GetByShareCodeAndUserId(userId, shareCode);
            
            if (existRequest != null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.ExistGroupJoinRequest;
                return response;
            }
            
            var groupExistUser = await _groupUserRepository.GetByGroupId(group.Id).FirstOrDefaultAsync(x => x.UserId == userId);

            if (groupExistUser != null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserAlreadyExist;
                return response;
            }
            
            var joinRequest = await _groupJoinRequestRepository.AddAsync(new GroupJoinRequest
            {
                FromUserId = userId,
                GroupShareCode = shareCode,
                IsActive = true,
            });
            response.Data = true;
            return response;
        }
        public async Task<BaseResponse<IEnumerable<GetGroupUsersInfoDto>>> GetGroupUsers(int groupId)
        {
            var response = new BaseResponse<IEnumerable<GetGroupUsersInfoDto>> {HasError = false, Data = null};
            
            if (groupId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupIdRequired;
                return response;
            }

            var groupUsers = _groupUserRepository.GetByGroupId(groupId);
            var users = _userRepository.GetUsersById(groupUsers.Select(x => x.UserId)).Select(x => new GetGroupUsersInfoDto
            {
                UserId = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();
            
            response.Data = users;
            return response;
        }
        public async Task<BaseResponse<bool>> ReplyGroupJoinRequestAsync(int requestId, int groupId, int adminId,bool isApproved)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            
            var joinRequest = await _groupJoinRequestRepository.GetByIdAsync(requestId);
            
            if (joinRequest == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.JoinRequestFail;
                return response;
            }

            if(!joinRequest.IsActive)
            {
                response.HasError = true;
                response.Error = ErrorCodes.JoinRequestFail;
                return response;
            }
            
            var group = await _groupRepository.GetGroupByShareCode(joinRequest.GroupShareCode);

            if (group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.JoinRequestFail;
                return response;
            }

            if (group.CreatedBy != adminId)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotAuthForOperation;
                return response;
            }
            
            if (isApproved)
            {
                await _groupUserRepository.AddAsync(new GroupUsers
                {
                    UserId = joinRequest.FromUserId,
                    GroupId = groupId,
                    
                });
            }
            
            joinRequest.IsActive = false;
            await _groupJoinRequestRepository.UpdateAsync(joinRequest);

            response.Data = true;
            return response;
        }

        #endregion
    }
}