using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Models.TransactionModels;
using PersonalFinanceManagement.Models;
using System.Linq;

namespace PersonalFinanceManagement.Database.Repositories.CategoryRepositories
{
    public class CategoryRepository : ICategoryRepository
    {
        PFMDbContext _dbContext;
        ILogger<CategoryEntity> _logger;

        public CategoryRepository(PFMDbContext dbContext, ILogger<CategoryEntity> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<string>> ImportCategoriesAsync(IEnumerable<CategoryEntity> categories)
        {
            List<string> badCategories = new List<string>();
            foreach (var category in categories)
            {
                var transactionToUpdate = _dbContext.Categories.FirstOrDefault(c => c.Code == category.Code);
                if (transactionToUpdate != null)
                {
                    transactionToUpdate.ParentCode = category.ParentCode;
                    transactionToUpdate.Name = category.Name;
                    _dbContext.Categories.Update(transactionToUpdate);
                }
                else
                {
                    await _dbContext.AddAsync(category);
                }
            }
            await _dbContext.SaveChangesAsync();
            return badCategories;
        }

        public async Task<List<CategoryEntity>> GetCategoriesAsync(string? parentCode)
        {
            var query = _dbContext.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(parentCode))
            {
                query = query.Where(x => x.ParentCode != null && x.ParentCode.Equals(parentCode));

            }

            return await query.ToListAsync();
        }
    }
}
