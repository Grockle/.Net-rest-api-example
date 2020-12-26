using BackendService.Common;
using BackendService.Context;
using BackendService.Data.Repository;
using BackendService.Data.Repository.Implementations;
using BackendService.Helpers;
using BackendService.Helpers.Implementations;
using BackendService.Hubs;
using BackendService.Services;
using BackendService.Services.Implementations;
using BackendService.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace BackendService.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddSwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo{ Title = "HomeKit Api", Version = "v1"});

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
 
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference 
                            { 
                                Type = ReferenceType.SecurityScheme, 
                                Id = "Bearer" 
                            }
                        },
                        new string[] {}
 
                    }
                });
            });
        }

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


        }

        public static void ServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            #region Services
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IHashService, HashService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<ICommonHelper, CommonHelper>(); 
            services.AddTransient<IPersonalService, PersonalService>(); 
            #endregion

            #region Repositories
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IGroupUserRepository, GroupUserRepository>();
            services.AddTransient<IGroupJoinRequestRepository, GroupJoinRequestRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IRelatedTransactionRepository, RelatedTransactionRepository>();
            services.AddTransient<IGroupBudgetBalanceRepository, GroupBudgetBalanceRepository>();
            services.AddTransient<IPersonalRepository, PersonalRepository>();
            #endregion
            
        }
    }
}
