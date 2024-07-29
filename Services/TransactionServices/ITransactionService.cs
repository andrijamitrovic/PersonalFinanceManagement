using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Services.TransactionServices
{
    public interface ITransactionService
    {
        Task<string> ImportTransactionsAsync(IFormFile file);
        Task<PagedSortedFilteredList<Transaction>> GetTransactionsAsync(List<Kind>? transactionKind, DateTime? startDate, DateTime? endDate, int page, int pageSize, SortOrder sortOrder, string? sortBy);
        Task<string> CategorizeTransactionAsync(string id, string catcode);
        Task SplitTransactionAsync(string id, List<Split> splits);
    }
}
