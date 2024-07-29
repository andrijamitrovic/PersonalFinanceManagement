using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.Database.Entities;

namespace PersonalFinanceManagement.Database.Configurations
{
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {
        public TransactionEntityTypeConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder.ToTable("transactions");
            // primary key
            builder.HasKey(x => x.Id);
            // definition of columns
            builder.Property(x => x.Id).IsRequired().HasMaxLength(64);
            builder.Property(x => x.BeneficiaryName).HasMaxLength(64);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Direction).HasConversion<string>().IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1024);
            builder.Property(x => x.Currency).HasMaxLength(3).IsFixedLength(true).IsRequired();
            builder.Property(x => x.Mcc);
            builder.Property(x => x.Kind).HasConversion<string>().IsRequired();
            builder.Property(x => x.CatCode).HasMaxLength(64);
            //definition of foreign keys
            builder.HasOne(c => c.SplitBy)
                   .WithMany(c => c.Splits)
                   .HasForeignKey(c => c.SplitId);
        }
    }
}
