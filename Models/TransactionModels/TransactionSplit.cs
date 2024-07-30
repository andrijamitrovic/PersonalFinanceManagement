namespace PersonalFinanceManagement.Models.TransactionModels
{
    public class TransactionSplit
    {
        public string Id { get; set; }
        public string Catcode { get; set; }
        public double Amount { get; set; }
        public string TransactionId { get; set; }
    }
    public class SplitsRequest
    {
        public List<TransactionSplit> Splits { get; set; }
    }
}
