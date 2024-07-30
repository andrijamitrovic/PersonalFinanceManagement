using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManagement.Database.Entities;

namespace PersonalFinanceManagement.Database.Configurations
{
    public class TransactionSplitEntityTypeConfiguration : IEntityTypeConfiguration<TransactionSplitEntity>
    {
        public TransactionSplitEntityTypeConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<TransactionSplitEntity> builder)
        {
            builder.ToTable("transactions_splits");
            // primary key
            builder.HasKey(x => x.Id);
            // definition of columns
            builder.Property(x => x.Id).IsRequired().HasMaxLength(64);
            builder.Property(x => x.CatCode).IsRequired().HasMaxLength(64);
            builder.Property(x => x.TransactionId).IsRequired().HasMaxLength(64);
            //definition of foreign keys
            builder.HasOne(t => t.Transaction)
                   .WithMany(t => t.TransactionSplits)
                   .HasForeignKey(t => t.TransactionId);
        }
    }
}
