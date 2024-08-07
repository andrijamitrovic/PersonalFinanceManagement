﻿using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;
using System.Collections;

namespace PersonalFinanceManagement.Database.Repositories.TransactionRepositories
{
    public interface ITransactionRepository
    {
        Task<PagedSortedFilteredList<TransactionEntity>> GetTransactionsAsync(List<Kind>? transactionKind, DateOnly? startDate, DateOnly? endDate, int page = 1, int pageSize = 10, SortOrder sortOrder = SortOrder.Asc, string? sortBy = null);

        Task<List<string>> ImportTransactionsAsync(IEnumerable<TransactionEntity> transaction);
        Task<BussinessProblem?> CategorizeTransactionAsync(string id, string catcode);
        Task<BussinessProblem?> SplitTransactionAsync(string id, SplitTransactionCommand splits);
        Task AutoCategorizeTransactionAsync(AutoCategorizeRuleList<Rule> rules);
    }
}
