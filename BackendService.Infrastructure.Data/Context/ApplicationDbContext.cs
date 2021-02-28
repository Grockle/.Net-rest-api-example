using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendService.Application.Interface.Helper;
using BackendService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Group = System.Text.RegularExpressions.Group;
using Transaction = System.Transactions.Transaction;

namespace BackendService.IoC.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeService _dateTime;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
        }

        #region Entity
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUsers> GroupUsers { get; set; }
        public DbSet<GroupJoinRequest> GroupJoinRequests { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RelatedTransaction> RelatedTransactions { get; set; }
        public DbSet<GroupBudgetBalance> GroupBudgetBalances { get; set; }
        public DbSet<GroupCategory> GroupCategories { get; set; }
        public DbSet<PersonalAccount> PersonalAccounts { get; set; }
        public DbSet<PersonalCategory> PersonalCategories { get; set; }
        public DbSet<PersonalTransaction> PersonalTransactions { get; set; }
        #endregion

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateTime = _dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdateTime = _dateTime.Now;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //All Decimals will have 18,6 Range
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,6)");
            }
            base.OnModelCreating(builder);
        }
    }
}
