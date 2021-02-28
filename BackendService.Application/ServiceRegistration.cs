using BackendService.Application.Common;
using BackendService.Application.Interface;
using BackendService.Application.Service;
using BackendService.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendService.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration config)
        {
            #region Services
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<ICommonHelper, CommonHelper>();
            services.AddTransient<IPersonalService, PersonalService>();
            services.AddTransient<IEmailVerificationService, EmailVerificationService>();
            services.AddTransient<IGroupCategoryService, GroupCategoryService>();
            services.AddTransient<IJoinRequestService, JoinRequestService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<IUserService, UserService>();
            #endregion
        }
    }
}