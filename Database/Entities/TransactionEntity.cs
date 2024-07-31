using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Database.Entities
{
    public class TransactionEntity
    {
        public string Id { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateOnly Date { get; set; }
        public Direction Direction { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public string Currency { get; set; }
        public Mcc? Mcc { get; set; }
        public Kind Kind { get; set; }
        public string? CatCode { get; set; }
        public CategoryEntity Category { get; set; }
        public ICollection<TransactionSplitEntity>? TransactionSplits { get; set; }
    }
}
