using BackendService.Domain.IRepository;
using BackendService.IoC.Data.Context;
using BackendService.IoC.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendService.IoC.Data
{
    public static class ServiceRegistration
    {
        public static void AddDataInfrastructure(this IServiceCollection services)
        {
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

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}