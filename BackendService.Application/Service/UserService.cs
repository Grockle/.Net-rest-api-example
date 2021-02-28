using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendService.Application.Constants;
using BackendService.Application.Interface;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.Group;
using BackendService.Application.Models.Responses.User;
using BackendService.Domain.IRepository;

namespace BackendService.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<UserInfoResponse>> InfoAsync(string token)
        {
            var response = new BaseResponse<UserInfoResponse> { HasError = false, Data = new UserInfoResponse() };
            if (string.IsNullOrEmpty(token))
            {
                response.HasError = true;
                response.Error = ErrorCodes.InvalidToken;
                return response;
            }
            var user = await _userRepository.GetUserByToken(token);

            if (user == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.InvalidToken;
                return response;
            }

            if (!user.EmailConfirmed)
            {
                return response;
            }

            response.Data = new UserInfoResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ShortName = (user.FirstName.Substring(0, 1) + user.LastName.Substring(0, 1)).ToUpperInvariant(),
                UserId = user.Id
            };
            return response;
        }
    }
}
