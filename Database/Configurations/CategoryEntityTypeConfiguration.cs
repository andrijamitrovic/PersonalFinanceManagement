using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.Database.Entities;

namespace PersonalFinanceManagement.Database.Configurations
{ 
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {
        public CategoryEntityTypeConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.ToTable("categories");
            // primary key
            builder.HasKey(x => x.Code);
            // definition of columns
            builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
            builder.Property(x => x.ParentCode).HasMaxLength(64);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
            //definition of foreign keys
            builder.HasMany(c => c.Transactions)
                   .WithOne(t => t.Category)
                   .HasForeignKey(t => t.CatCode);
            builder.HasOne(c => c.ParentCategory)
                   .WithMany(c => c.ChildCategories)
                   .HasForeignKey(c => c.ParentCode);
        }
    }
}
