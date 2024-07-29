﻿
using PersonalFinanceManagement.Models.CategoryModels;
using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<string> ImportCategoriesAsync(IFormFile file);
        Task<List<Category>> GetCategoriesAsync(string? parentCategory);
        Task<List<SpendingAnalytics>> GetSpendingAnalyticsAsync(string? catcode, DateTime? startDate, DateTime? endDate, Direction? direction);
    }
}