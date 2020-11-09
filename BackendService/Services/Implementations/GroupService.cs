using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Group.Request;
using BackendService.Data.DTOs.Group.Response;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;
using BackendService.Data.Repository;
using BackendService.Helpers;
using BackendService.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IEmailService _emailService;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IGroupJoinRequestRepository _groupJoinRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;

        public GroupService(IHashService hashService, IDateTimeService dateTimeService, IGroupRepository groupRepository, IEmailService emailService, IGroupUserRepository groupUserRepository, IGroupJoinRequestRepository groupJoinRequestRepository, IUserRepository userRepository)
        {
            _hashService = hashService;
            _dateTimeService = dateTimeService;
            _groupRepository = groupRepository;
            _emailService = emailService;
            _groupUserRepository = groupUserRepository;
            _groupJoinRequestRepository = groupJoinRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<bool>> AddGroupAsync(AddGroupRequest model)
        {
            if (model.UserId == 0)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "UserId is required", false);
            }
            
            var existGroup = await _groupRepository.GetGroupWithSameNameAsync(model.GroupName, model.UserId);
            
            if (existGroup != null)
            {
               return new GeneralMapping<bool>().MapBaseResponse(true, "Group name already exist", false);
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
                return new GeneralMapping<bool>().MapBaseResponse(false, "", true);
            }
            
            await _groupRepository.DeleteAsync(group);
            return new GeneralMapping<bool>().MapBaseResponse(true, "Unable to add group", false);
        }

        public async Task<BaseResponse<List<GetUserGroupsDto>>> GetUserGroupsAsync(int userId)
        {
            if (userId == 0)
            {
                return new GeneralMapping<List<GetUserGroupsDto>>().MapBaseResponse(true, "UserId is required ", null);
            }

            var groups = _groupRepository.GetGroupsByUserId(userId);
            
            return new GeneralMapping<List<GetUserGroupsDto>>().MapBaseResponse(false, "", groups);
        }
        
        public async Task<BaseResponse<List<GetGroupJoinRequestsDto>>> GetGroupJoinRequests(string shareCode)
        {
            if (string.IsNullOrEmpty(shareCode))
            {
                return new GeneralMapping<List<GetGroupJoinRequestsDto>>().MapBaseResponse(true, "Share code is required", null);
            }
            var groupJoinRequests = _groupJoinRequestRepository.GetRequestsByShareCode(shareCode);
            
            if (groupJoinRequests == null)
            {
                return new GeneralMapping<List<GetGroupJoinRequestsDto>>().MapBaseResponse(false, "No request", null);
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
            
            return new GeneralMapping<List<GetGroupJoinRequestsDto>>().MapBaseResponse(false, "", users);
        }
        
        public async Task<BaseResponse<bool>>SendGroupJoinRequest(int userId, string shareCode)
        {
            if (userId == 0)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "UserId is required", false);
            }

            var group = await _groupRepository.GetGroupByShareCode(shareCode);

            if (group == null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Group is not exist", false);
            }

            var existRequest = await _groupJoinRequestRepository.GetByShareCodeAndUserId(userId, shareCode);
            
            if (existRequest != null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "You send request already", false);
            }
            
            var groupExistUser = await _groupUserRepository.GetByGroupId(group.Id).FirstOrDefaultAsync(x => x.UserId == userId);

            if (groupExistUser != null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "User already exist", false);
            }
            
            var joinRequest = await _groupJoinRequestRepository.AddAsync(new GroupJoinRequest
            {
                FromUserId = userId,
                GroupShareCode = shareCode,
                IsActive = true,
            });
            
            return new GeneralMapping<bool>().MapBaseResponse(false, "Request send", true);
        }

        public async Task<BaseResponse<List<GetGroupUsersInfoDto>>> GetGroupUsers(int groupId)
        {
            if (groupId == 0)
            {
                return new GeneralMapping<List<GetGroupUsersInfoDto>>().MapBaseResponse(true, "Group Id is required", null);
            }

            var groupUsers = _groupUserRepository.GetByGroupId(groupId);
            var users = _userRepository.GetUsersById(groupUsers.Select(x => x.UserId)).Select(x => new GetGroupUsersInfoDto
            {
                UserId = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();
            
            return new GeneralMapping<List<GetGroupUsersInfoDto>>().MapBaseResponse(false, "", users);
        }

        public async Task<BaseResponse<bool>> ReplyGroupJoinRequestAsync(int requestId, int groupId, int adminId,bool isApproved)
        {
            var joinRequest = await _groupJoinRequestRepository.GetByIdAsync(requestId);
            
            if (joinRequest == null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Wrong request", false);
            }

            if(!joinRequest.IsActive)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Wrong request", false);
            }
            
            var group = await _groupRepository.GetGroupByShareCode(joinRequest.GroupShareCode);

            if (group == null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Wrong request", false);
            }

            if (group.CreatedBy != adminId)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "You are not authorized to approve", false);
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
            
            return new GeneralMapping<bool>().MapBaseResponse(false, "Operation is success", true);
        }
    }
}