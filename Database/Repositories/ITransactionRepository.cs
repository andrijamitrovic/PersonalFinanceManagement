using PersonalFinanceManagement.Database.Entities;
using System.Collections;

namespace PersonalFinanceManagement.Database.Repositories
{
    public interface ITransactionRepository
    {
        Task<bool> CreateTransaction(TransactionEntity transaction);
    }
}
