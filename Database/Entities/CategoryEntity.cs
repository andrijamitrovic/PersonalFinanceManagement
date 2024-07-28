namespace PersonalFinanceManagement.Database.Entities
{
    public class CategoryEntity
    {
        public string Code { get; set; }
        public string? ParentCode { get; set; }
        public string Name { get; set; }
        public ICollection<TransactionEntity> Transactions { get; set; }
        public CategoryEntity ParentCategory { get; set; }
        public ICollection<CategoryEntity> ChildCategories { get; set; }
    }
}
