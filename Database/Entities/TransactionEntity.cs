using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Database.Entities
{
    public class TransactionEntity
    {
        public string Id { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public Direction Direction { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public string Currency { get; set; }
        public int? Mcc { get; set; }
        public Kind Kind { get; set; }
        public string? CatCode { get; set; }
        public string? SplitId { get; set; }
        public CategoryEntity Category { get; set; }
        public TransactionEntity SplitBy { get; set; }
        public ICollection<TransactionEntity>? Splits { get; set; }
    }
}
