using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendService.Common;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.User.Request;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;
using BackendService.Data.Enums;
using BackendService.Data.Repository;
using BackendService.Helpers;
using BackendService.Models;

namespace BackendService.Services.Implementations
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
            var response = new BaseResponse<RegisterUserResponse> {HasError = false, Data = new RegisterUserResponse()};
            
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

            var userDo =  await _userRepository.AddAsync(user);

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
        public async Task<BaseResponse<LoginResponse>> ConfirmEmail(ConfirmEmailRequest confirmModel)
        {
            var response = new BaseResponse<LoginResponse> {HasError = false, Data = new LoginResponse()};
            
            var user = await _userRepository.GetUserByEmailAsync(confirmModel.Email);
            
            var confirmationApproved = await _userRepository.ControlVerification(confirmModel.Email, confirmModel.Code);

            if (confirmationApproved && user != null)
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
                    UserId = user.Id
                };
                return response;
            }

            response.HasError = true;
            response.Error = ErrorCodes.EmailConfirmFail;
            return response;
        }
        public async Task<BaseResponse<bool>> UpdateVerificationCodeAsync(UpdateEmailVerificationCodeRequest model)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            
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
        public async Task<BaseResponse<LoginResponse>> Authenticate(string email, string password)
        {
            var response = new BaseResponse<LoginResponse> {HasError = false, Data = new LoginResponse()};
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
                Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, IsLoggedIn = true,
                IsEmailConfirmed = user.EmailConfirmed, UserId = user.Id, Token = user.Token
            };
            return response;
        }
        public async Task<BaseResponse<LoginResponse>> LoginWithTokenAsync(string token)
        {
            var response = new BaseResponse<LoginResponse> {HasError = false, Data = new LoginResponse()};
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
                response.Data.IsLoggedIn = false;
                response.Data.IsEmailConfirmed = false;
                return response;
            }

            response.Data = new LoginResponse
            {
                IsLoggedIn = true,
                IsEmailConfirmed = true,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ShortName = (user.FirstName.Substring(0, 1) + user.LastName.Substring(0, 1)).ToUpperInvariant(),
                UserId = user.Id
            };
            return response;
        }
        public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest loginModel)
        {
            var response = new BaseResponse<LoginResponse> {HasError = false, Data = new LoginResponse()};
            var user = await _userRepository.GetUserByEmailAsync(loginModel.Email);
           
            if (user == null)
            {
                response.Error = ErrorCodes.WrongEmailPassword;
                response.HasError = true;
                return response;
            }
            
            var passwordMatched = _hashService.DecryptString(user.PasswordHash) == loginModel.Password;
            
            if (passwordMatched)
            {
                if (!user.EmailConfirmed)
                {
                    response.Data.IsLoggedIn = false;
                    response.Data.IsEmailConfirmed = false;
                    return response;
                }

                response.Data = new LoginResponse
                {
                    IsLoggedIn = true,
                    IsEmailConfirmed = true,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ShortName = (user.FirstName.Substring(0, 1) + user.LastName.Substring(0, 1)).ToUpperInvariant(),
                    UserId = user.Id
                };
                return response;
            }

            response.HasError = true;
            response.Error = ErrorCodes.WrongEmailPassword;
            return response;
        }
        public async Task<BaseResponse<bool>> ControlResetCodeAsync(string email, string resetCode)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
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
        public async Task<BaseResponse<bool>> SendPasswordResetRequestAsync(string email)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
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
        public async Task<BaseResponse<bool>> ChangePasswordAsync(ResetPasswordRequest resetRequestModel)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
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
        
        #endregion

    }
}