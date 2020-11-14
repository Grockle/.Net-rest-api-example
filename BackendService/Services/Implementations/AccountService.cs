using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.User.Request;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;
using BackendService.Data.Repository;
using BackendService.Helpers;
using BackendService.Mappings;
using BackendService.Models;
using BackendService.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IDateTimeService _dateTimeService;
        private readonly JwtSettings _jwtSettings;

        public AccountService(IEmailService emailService, IUserRepository userRepository, IHashService hashService, IDateTimeService dateTimeService, IOptions<JwtSettings> appSettings)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _hashService = hashService;
            _dateTimeService = dateTimeService;
            _jwtSettings = appSettings.Value;
        }

        #region Services
        public async Task<BaseResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest userModel)
        {
            var existEmail = await _userRepository.EmailExist(userModel.Email);

            if (existEmail)
            {
                return new GeneralMapping<RegisterUserResponse>().MapBaseResponse(true, "Email is already exist", 
                    new RegisterUserResponse{ IsRegistered = false , Email = string.Empty});
            }
            
            var user = new User
            {
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                EmailConfirmed = false,
                PasswordHash = _hashService.EncryptString(userModel.Password),
                EmailVerificationCode = GenerateCode(),
                VerificationEndTime = _dateTimeService.Now.AddHours(1),
                SecretEmail = SetSecretEmail(userModel.Email),
                AccessFailedCount = 0,
                PasswordResetCode = null,
                ResetCodeEndTime = null
            };

            var response =  await _userRepository.AddAsync(user);

            try
            {
                await _emailService.SendAsync(new EmailRequest
                {
                    To = response.Email,
                    Body =
                        $"Hello {userModel.FirstName} {userModel.LastName}. Welcome to MATASIS. Email verification code : {response.EmailVerificationCode}",
                    Subject = "Welcome Message"
                });
            }
            catch (Exception)
            {
                await _userRepository.DeleteAsync(response);
                return new GeneralMapping<RegisterUserResponse>().MapBaseResponse(true, "Error occurred during the registration process", 
                    new RegisterUserResponse{IsRegistered = false, Email = string.Empty});
            }
            
            return new GeneralMapping<RegisterUserResponse>().MapBaseResponse(false, string.Empty,  new RegisterUserResponse
            {
                Email = userModel.Email,
                IsRegistered = true
            }); 
        }
        public async Task<BaseResponse<LoginResponse>> ConfirmEmail(ConfirmEmailRequest confirmModel)
        {
            var user = await _userRepository.GetUserByEmailAsync(confirmModel.Email);
            
            var confirmationApproved = await _userRepository.ControlVerification(confirmModel.Email, confirmModel.Code);

            if (confirmationApproved && user != null)
            {
                user.EmailConfirmed = true;
                user.EmailVerificationCode = null;
                user.VerificationEndTime = DateTime.Now;
                await _userRepository.UpdateAsync(user);
                return new BaseResponse<LoginResponse>
                {
                    HasError = false,
                    Message = "",
                    Data = new LoginResponse
                    {
                        IsEmailConfirmed = true,
                        IsLoggedIn = true,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        ShortName = (user.FirstName.Substring(0, 1) + user.LastName.Substring(0, 1)).ToUpperInvariant(),
                        UserId = user.Id
                    }
                };
            }
            
            return new BaseResponse<LoginResponse>
            {
                HasError = true,
                Message = "Email confirmation failed",
                Data = new LoginResponse
                {
                    IsEmailConfirmed = false,
                    IsLoggedIn = false
                }
            };
        }
        public async Task<BaseResponse<bool>> UpdateVerificationCodeAsync(UpdateEmailVerificationCodeRequest model)
        {
            var user = await _userRepository.GetUserWithoutEmailConfirmedAsync(model.Email,
                _hashService.EncryptString(model.Password));

            if (user == null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Update email confirmation code failed", false);  
            }

            user.EmailVerificationCode = GenerateCode();
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
                
                return new GeneralMapping<bool>().MapBaseResponse(false, "Update email confirmation code success", true); 
            }
            catch (Exception)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Error occurred during the updating email verification code", false); 
            }
        }
        public async Task<BaseResponse<LoginResponse>> Authenticate(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return new GeneralMapping<LoginResponse>().MapBaseResponse(true, $"No Accounts Registered with {email}.", null);
            }
            
            var passwordMatched = _hashService.DecryptString(user.PasswordHash) == password;
            
            if (!passwordMatched)
            {
                return new GeneralMapping<LoginResponse>().MapBaseResponse(true, $"Invalid Credentials for '{email}'.", null);
            }
            
            // Authentication(Yetkilendirme) başarılı ise JWT token üretilir.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            await _userRepository.UpdateAsync(user);


            return new GeneralMapping<LoginResponse>().MapBaseResponse(false,"",new LoginResponse
            {
                Email = user.Email, FirstName = user.FirstName, LastName = user.LastName,IsLoggedIn = true, IsEmailConfirmed = user.EmailConfirmed,UserId = user.Id, Token = user.Token
            });
        }
        public async Task<BaseResponse<LoginResponse>> LoginWithTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new GeneralMapping<LoginResponse>().MapBaseResponse(false, "Invalid Token", new LoginResponse{IsLoggedIn = false, IsEmailConfirmed = false});
            }
            var user = await _userRepository.GetUserByToken(token);
           
            if (user == null)
            {
                return new GeneralMapping<LoginResponse>().MapBaseResponse(false, "Invalid Token", new LoginResponse{IsLoggedIn = false, IsEmailConfirmed = false});
            }
            
            if (!user.EmailConfirmed)
            {
                return new GeneralMapping<LoginResponse>().MapBaseResponse(false, "Email is not confirmed", new LoginResponse{IsLoggedIn = true, IsEmailConfirmed = false});
            }
            
            return new GeneralMapping<LoginResponse>().MapBaseResponse(false, "Login Success", new LoginResponse
            {
                IsLoggedIn = true, 
                IsEmailConfirmed = true,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ShortName = (user.FirstName.Substring(0, 1) + user.LastName.Substring(0, 1)).ToUpperInvariant(),
                UserId = user.Id
            });
        }
        public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest loginModel)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginModel.Email);
           
            if (user == null)
            {
                return new GeneralMapping<LoginResponse>().MapBaseResponse(true, "Wrong Email or Password", new LoginResponse{IsLoggedIn = false, IsEmailConfirmed = false});
            }
            
            var passwordMatched = _hashService.DecryptString(user.PasswordHash) == loginModel.Password;
            
            if (passwordMatched)
            {
                if (!user.EmailConfirmed)
                {
                    return new GeneralMapping<LoginResponse>().MapBaseResponse(false, "Email is not confirmed", new LoginResponse{IsLoggedIn = true, IsEmailConfirmed = false});
                }
                return new GeneralMapping<LoginResponse>().MapBaseResponse(false, "Login Success", new LoginResponse
                {
                    IsLoggedIn = true, 
                    IsEmailConfirmed = true,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ShortName = (user.FirstName.Substring(0, 1) + user.LastName.Substring(0, 1)).ToUpperInvariant(),
                    UserId = user.Id
                });
            }
            
            return new GeneralMapping<LoginResponse>().MapBaseResponse(false, "Wrong Email or Password", new LoginResponse{IsLoggedIn = false, IsEmailConfirmed = false});
        }
        public async Task<BaseResponse<bool>> ControlResetCodeAsync(string email, string resetCode)
        {
            var resetCodeCorrect = await _userRepository.ControlPasswordResetCodeAsync(email, resetCode);

            if (resetCodeCorrect)
            {
                return new GeneralMapping<bool>().MapBaseResponse(false, "Success", true);
            }
            
            return new GeneralMapping<bool>().MapBaseResponse(false, "Reset code not valid", false);
        }
        public async Task<BaseResponse<bool>> SendPasswordResetRequestAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(false, "Reset code request not success", false);
            }

            user.ResetCodeEndTime = _dateTimeService.Now.AddHours(1);
            user.PasswordResetCode = GenerateCode();
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
                
                return new GeneralMapping<bool>().MapBaseResponse(false, "Password reset code is send your email", true); 
            }
            catch (Exception)
            {
                return new GeneralMapping<bool>().MapBaseResponse(true, "Reset code request not success", false); 
            }
        }
        public async Task<BaseResponse<bool>> ChangePasswordAsync(ResetPasswordRequest resetRequestModel)
        {
            var user = await _userRepository.GetUserByEmailAsync(resetRequestModel.Email);

            if (user == null)
            {
                return new GeneralMapping<bool>().MapBaseResponse(false, "Change password failed", false);
            }
            
            var resetCodeCorrect = await _userRepository.ControlPasswordResetCodeAsync(resetRequestModel.Email, resetRequestModel.Code);
            
            if (resetCodeCorrect)
            {
                user.PasswordHash = _hashService.EncryptString(resetRequestModel.Password);
                user.ResetCodeEndTime = null;
                user.PasswordResetCode = null;
                await _userRepository.UpdateAsync(user);
                return new GeneralMapping<bool>().MapBaseResponse(false, "Your password changed successfully", true);
            }
            
            return new GeneralMapping<bool>().MapBaseResponse(false, "Change password failed", false);
        }
        #endregion

        #region HelperFunctions
        private string GenerateCode()
        {
            Random generator = new Random();
            String randomCode = generator.Next(0, 999999).ToString("D6");
            return randomCode;
        }
        private string SetSecretEmail(string email)
        {
            var splits = email.Split("@");
            return splits[0][0] + "***@" + splits[1];
        }
        
        #endregion
        
    }
}