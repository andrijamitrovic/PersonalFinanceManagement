using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.Database.Configurations;
using PersonalFinanceManagement.Database.Entities;

namespace PersonalFinanceManagement.Database
{
    public class PFMDbContext : DbContext
    {
        public PFMDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(
               new TransactionEntityTypeConfiguration()
                );
            modelBuilder.ApplyConfiguration(
               new CategoryEntityTypeConfiguration()
               );
            base.OnModelCreating(modelBuilder);
        }
    }
}
