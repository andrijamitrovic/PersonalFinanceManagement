
using PersonalFinanceManagement.Models.CategoryModels;

namespace PersonalFinanceManagement.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<string> ImportCategoriesAsync(IFormFile file);
        Task<List<Category>> GetCategoriesAsync(string? parentCategory);
    }
}
