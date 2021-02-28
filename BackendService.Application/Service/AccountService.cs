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
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;

namespace BackendService.Application.Service
{
    public class AccountService : IAccountService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICommonHelper _commonHelper;

        public AccountService(IEmailService emailService, IUserRepository userRepository, IHashService hashService, IDateTimeService dateTimeService, ICommonHelper commonHelper)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _hashService = hashService;
            _dateTimeService = dateTimeService;
            _commonHelper = commonHelper;
        }

        #region Services
        public async Task<BaseResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest userModel)
        {
            var response = new BaseResponse<RegisterUserResponse> { HasError = false, Data = new RegisterUserResponse() };

            var existEmail = await _userRepository.EmailExist(userModel.Email);

            if (existEmail)
            {
                response.Error = ErrorCodes.EmailExist;
                response.HasError = true;
                return response;
            }

            var user = new User
            {
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                EmailConfirmed = false,
                PasswordHash = _hashService.EncryptString(userModel.Password),
                EmailVerificationCode = _commonHelper.GenerateCode(),
                VerificationEndTime = _dateTimeService.Now.AddHours(1),
                SecretEmail = _commonHelper.SetSecretEmail(userModel.Email),
                AccessFailedCount = 0,
                PasswordResetCode = null,
                ResetCodeEndTime = null
            };

            var userDo = await _userRepository.AddAsync(user);

            try
            {
                await _emailService.SendAsync(new EmailRequest
                {
                    To = userDo.Email,
                    Body =
                        $"Hello {userModel.FirstName} {userModel.LastName}. Welcome to MATASIS. Email verification code : {userDo.EmailVerificationCode}",
                    Subject = "Welcome Errors"
                });
            }
            catch (Exception)
            {
                await _userRepository.DeleteAsync(userDo);
                response.HasError = true;
                response.Error = ErrorCodes.CommonProcessError;
                return response;
            }

            response.Data = new RegisterUserResponse
            {
                Email = userModel.Email,
                IsRegistered = true
            };

            return response;
        }
        public async Task<BaseResponse<LoginResponse>> Authenticate(string email, string password)
        {
            var response = new BaseResponse<LoginResponse> { HasError = false, Data = new LoginResponse() };
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.WrongEmailPassword;
                return response;
            }

            var passwordMatched = _hashService.DecryptString(user.PasswordHash) == password;

            if (!passwordMatched)
            {
                response.HasError = true;
                response.Error = ErrorCodes.WrongEmailPassword;
                return response;
            }

            // Authentication(Yetkilendirme) başarılı ise JWT token üretilir.
            user.Token = _commonHelper.GenerateJwtToken(user.Id.ToString());

            await _userRepository.UpdateAsync(user);

            response.Data = new LoginResponse
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsLoggedIn = true,
                IsEmailConfirmed = user.EmailConfirmed,
                UserId = user.Id,
                Token = user.Token
            };
            return response;
        }
        #endregion

    }
}