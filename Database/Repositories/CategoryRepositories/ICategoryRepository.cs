using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Database.Repositories.CategoryRepositories
{
    public interface ICategoryRepository
    {
        Task<List<string>> ImportCategoriesAsync(IEnumerable<CategoryEntity> categories);
        public Task<List<CategoryEntity>> GetCategoriesAsync(string? parentCode);
        Task<List<SpendingAnalytics>> GetSpendingAnalyticsAsync(string? catcode, DateTime? startDate, DateTime? endDate, Direction? direction);
    }
}
