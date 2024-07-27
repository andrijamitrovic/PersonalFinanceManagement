using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Services
{
    public interface ITransactionService
    {
        Task<string> ImportTransactions(IFormFile file);
        Task<PagedSortedFilteredList<Transaction>> GetTransactionsAsync(List<Kind>? transactionKind, DateTime? startDate, DateTime? endDate, int page, int pageSize, SortOrder sortOrder, string? sortBy);
    }
}
