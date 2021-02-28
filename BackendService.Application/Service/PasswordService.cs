using System;
using System.Threading.Tasks;
using BackendService.Application.Common;
using BackendService.Application.Constants;
using BackendService.Application.Interface;
using BackendService.Application.Interface.Helper;
using BackendService.Application.Models.Requests.General;
using BackendService.Application.Models.Requests.User;
using BackendService.Application.Models.Responses.General;
using BackendService.Domain.IRepository;

namespace BackendService.Application.Service
{
    public class PasswordService : IPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IEmailService _emailService;
        private readonly ICommonHelper _commonHelper;

        public PasswordService(IUserRepository userRepository, ICommonHelper commonHelper, IDateTimeService dateTimeService, IHashService hashService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _commonHelper = commonHelper;
            _dateTimeService = dateTimeService;
            _hashService = hashService;
            _emailService = emailService;
        }

        public async Task<BaseResponse<bool>> ControlAsync(string email, string resetCode)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var resetCodeCorrect = await _userRepository.ControlPasswordResetCodeAsync(email, resetCode);

            if (resetCodeCorrect)
            {
                response.Data = true;
                return response;
            }

            response.HasError = true;
            response.Error = ErrorCodes.ResetCodeNotValid;
            return response;
        }

        public async Task<BaseResponse<bool>> ResetAsync(string email)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.SendPasswordResetCodeNotSuccess;
                return response;
            }

            user.ResetCodeEndTime = _dateTimeService.Now.AddHours(1);
            user.PasswordResetCode = _commonHelper.GenerateCode();
            await _userRepository.UpdateAsync(user);

            try
            {
                await _emailService.SendAsync(new EmailRequest
                {
                    To = user.Email,
                    Body =
                        $"Your password reset code: {user.PasswordResetCode}",
                    Subject = "Reset Password"
                });

                response.Data = true;
                return response;
            }
            catch (Exception)
            {
                response.HasError = true;
                response.Error = ErrorCodes.SendPasswordResetCodeNotSuccess;
                return response;
            }
        }

        public async Task<BaseResponse<bool>> ChangeAsync(ResetPasswordRequest resetRequestModel)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
            var user = await _userRepository.GetUserByEmailAsync(resetRequestModel.Email);

            if (user == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.ChangePasswordFailed;
                return response;
            }

            var resetCodeCorrect = await _userRepository.ControlPasswordResetCodeAsync(resetRequestModel.Email, resetRequestModel.Code);

            if (resetCodeCorrect)
            {
                user.PasswordHash = _hashService.EncryptString(resetRequestModel.Password);
                user.ResetCodeEndTime = null;
                user.PasswordResetCode = null;
                await _userRepository.UpdateAsync(user);
                response.Data = true;
                return response;
            }
            response.HasError = true;
            response.Error = ErrorCodes.ChangePasswordFailed;
            return response;
        }

    }
}
