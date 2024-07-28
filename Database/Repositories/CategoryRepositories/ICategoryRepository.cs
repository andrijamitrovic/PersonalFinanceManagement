using PersonalFinanceManagement.Database.Entities;

namespace PersonalFinanceManagement.Database.Repositories.CategoryRepositories
{
    public interface ICategoryRepository
    {
        Task<List<string>> ImportCategoriesAsync(IEnumerable<CategoryEntity> categories);
        public Task<List<CategoryEntity>> GetCategoriesAsync(string? parentCode);
    }
}
