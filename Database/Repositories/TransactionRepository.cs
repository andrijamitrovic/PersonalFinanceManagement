using PersonalFinanceManagement.Database.Entities;

namespace PersonalFinanceManagement.Database.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        TransactionDbContext _dbContext;

        public TransactionRepository(TransactionDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateTransaction(TransactionEntity transaction)
        {
            var transactionById = _dbContext.Transactions.FirstOrDefault(trans => trans.Id == transaction.Id);
            if (transactionById == null)
            {
                await _dbContext.AddAsync(transaction);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
