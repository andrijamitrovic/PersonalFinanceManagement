namespace PersonalFinanceManagement.Models.TransactionModels
{
    public class Split
    {
        public string Catcode { get; set; }
        public double Amount { get; set; }
    }
    public class SplitsRequest
    {
        public List<Split> Splits { get; set; }
    }
}
