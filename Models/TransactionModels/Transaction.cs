using CsvHelper.Configuration.Attributes;

namespace PersonalFinanceManagement.Models.TransactionModels
{
    public class Transaction
    {
        public string Id { get; set; }
        public string? BeneficiaryName { get; set; }
        public string Date { get; set; }
        [EnumIgnoreCase]
        public Direction Direction { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public string Currency { get; set; }
        public int? Mcc { get; set; }
        [EnumIgnoreCase]
        public Kind Kind { get; set; }
    }
}
