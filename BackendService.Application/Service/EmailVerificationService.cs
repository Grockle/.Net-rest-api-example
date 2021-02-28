using System;
using System.Threading.Tasks;
using BackendService.Application.Common;
using BackendService.Application.Constants;
using BackendService.Application.Interface;
using BackendService.Application.Interface.Helper;
using BackendService.Application.Models.Requests.General;
using BackendService.Application.Models.Requests.User;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.User;
using BackendService.Domain.IRepository;

namespace BackendService.Application.Service
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICommonHelper _commonHelper;

        public EmailVerificationService(IDateTimeService dateTimeService, IHashService hashService, IUserRepository userRepository, IEmailService emailService, ICommonHelper commonHelper)
        {
            _dateTimeService = dateTimeService;
            _hashService = hashService;
            _userRepository = userRepository;
            _emailService = emailService;
            _commonHelper = commonHelper;
        }

        public async Task<BaseResponse<LoginResponse>> ConfirmAsync(ConfirmEmailRequest confirmModel, string token)
        {
            var response = new BaseResponse<LoginResponse> { HasError = false, Data = new LoginResponse() };

            var user = await _userRepository.GetUserByEmailAsync(confirmModel.Email);

            if (user != null)
            {
                if (user.Token == null || (user.Token != null && user.Token != token))
                {
                    response.HasError = true;
                    response.Error = ErrorCodes.InvalidToken;
                    return response;
                }

                var confirmationApproved = await _userRepository.ControlVerification(confirmModel.Email, confirmModel.Code);

                if (confirmationApproved)
                {
                    user.EmailConfirmed = true;
                    user.EmailVerificationCode = null;
                    user.VerificationEndTime = DateTime.Now;
                    await _userRepository.UpdateAsync(user);

                    response.Data = new LoginResponse
                    {
                        IsEmailConfirmed = true,
                        IsLoggedIn = true,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        ShortName = (user.FirstName.Substring(0, 1) + user.LastName.Substring(0, 1)).ToUpperInvariant(),
                        UserId = user.Id,
                        Token = user.Token
                    };
                    return response;
                }
            }

            response.HasError = true;
            response.Error = ErrorCodes.EmailConfirmFail;
            return response;
        }
        public async Task<BaseResponse<bool>> UpdateAsync(UpdateEmailVerificationCodeRequest model)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };

            var user = await _userRepository.GetUserWithoutEmailConfirmedAsync(model.Email,
                _hashService.EncryptString(model.Password));

            if (user == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UpdateEmailConfirmCodeFail;
                return response;
            }

            user.EmailVerificationCode = _commonHelper.GenerateCode();
            user.VerificationEndTime = _dateTimeService.Now.AddHours(1);
            await _userRepository.UpdateAsync(user);

            try
            {
                await _emailService.SendAsync(new EmailRequest
                {
                    To = user.Email,
                    Body =
                        $"Email verification code : {user.EmailVerificationCode}",
                    Subject = "Verification Code"
                });

                response.Data = true;
                return response;
            }
            catch (Exception)
            {
                response.HasError = true;
                response.Error = ErrorCodes.CommonProcessError;
                return response;
            }
        }
    }
}
