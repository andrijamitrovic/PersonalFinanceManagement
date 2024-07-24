using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Services
{
    public interface ITransactionService
    {
        Task<bool> ImportTransactions(IFormFile file);
    }
}
