using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.Database.Configurations;
using PersonalFinanceManagement.Database.Entities;

namespace PersonalFinanceManagement.Database
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<TransactionEntity> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(
               new TransactionEntityTypeConfiguration()
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
