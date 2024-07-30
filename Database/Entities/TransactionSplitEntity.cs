namespace PersonalFinanceManagement.Database.Entities
{
    public class TransactionSplitEntity
    {
        public string Id { get; set; }
        public string CatCode { get; set; }
        public double Amount { get; set; }
        public string TransactionId { get; set; }
        public TransactionEntity Transaction { get; set; }
    }
}
