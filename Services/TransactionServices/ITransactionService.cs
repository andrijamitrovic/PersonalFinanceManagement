using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Services.TransactionServices
{
    public interface ITransactionService
    {
        Task<string> ImportTransactionsAsync(IFormFile file);
        Task<PagedSortedFilteredList<Transaction>> GetTransactionsAsync(List<Kind>? transactionKind, DateOnly? startDate, DateOnly? endDate, int page, int pageSize, SortOrder sortOrder, string? sortBy);
        Task<BussinessProblem?> CategorizeTransactionAsync(string id, string catcode);
        Task<BussinessProblem?> SplitTransactionAsync(string id, SplitTransactionCommand splits);
        Task AutoCategorizeTransactionAsync();
    }
}
