using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Models.TransactionModels;
using PersonalFinanceManagement.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Globalization;

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

        public async Task<List<SpendingAnalytics>> GetSpendingAnalyticsAsync(string? catcode, DateTime? startDate, DateTime? endDate, Direction? direction)
        {

            var query = _dbContext.Transactions.AsQueryable();
            query = query.Where(t => !string.IsNullOrWhiteSpace(t.CatCode));
            if (!string.IsNullOrEmpty(catcode))
            {
                var listOfCatCodes = new List<string> { catcode };
                listOfCatCodes.AddRange(await GetAllCategoriesByCatCode(catcode));
                query = query.Where(t => listOfCatCodes.Contains(t.CatCode));
            }
            if (startDate != null)
            {
                query = query.Where(x => x.Date > startDate);
            }
            if (endDate != null)
            {
                query = query.Where(x => x.Date < endDate);
            }
            if (direction != null)
            {
                query = query.Where(x => x.Direction == direction);
            }

            var groups = (await query.ToListAsync())
                                   .GroupBy(t => t.CatCode)
                                   .Select(g => new SpendingAnalytics
                                   {
                                        Catcode = g.Key,
                                        Amount = g.Sum(t => t.Amount),
                                        Count = g.Count()
                                   })
                                   .ToList();

            return groups;
        }

        private async Task<List<string>> GetAllCategoriesByCatCode(string catcode)
        {
            var listOfChildren = new List<string>();
            var children = await _dbContext.Categories.Where(c => c.ParentCode == catcode).ToListAsync();
            foreach (var child in children)
            {
                listOfChildren.Add(child.Code);
                listOfChildren.AddRange(await GetAllCategoriesByCatCode(child.Code));
            }
            return listOfChildren;
        }
    }
}
