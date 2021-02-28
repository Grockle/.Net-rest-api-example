using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendService.Application.Constants;
using BackendService.Application.Interface;
using BackendService.Application.Models.Responses.General;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;

namespace BackendService.Application.Service
{
    public class JoinRequestService : IJoinRequestService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IGroupJoinRequestRepository _groupJoinRequestRepository;

        public JoinRequestService(IGroupRepository groupRepository, IGroupUserRepository groupUserRepository, IGroupJoinRequestRepository groupJoinRequestRepository)
        {
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
            _groupJoinRequestRepository = groupJoinRequestRepository;
        }

        public async Task<BaseResponse<bool>> SendAsync(int userId, string shareCode)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
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

            var groupUsers = await _groupUserRepository.GetByGroupId(@group.Id);
            var groupExistUser = groupUsers.FirstOrDefault(x => x.Id == userId);

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

        public async Task<BaseResponse<bool>> ReplyAsync(int requestId, int groupId, int adminId, bool isApproved)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };

            var joinRequest = await _groupJoinRequestRepository.GetByIdAsync(requestId);

            if (joinRequest == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.JoinRequestFail;
                return response;
            }

            if (!joinRequest.IsActive)
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
    }
}
