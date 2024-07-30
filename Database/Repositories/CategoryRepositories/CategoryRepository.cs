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
            var analytics = new List<SpendingAnalytics>();

            if (!string.IsNullOrEmpty(catcode))
            {
                analytics.Add(await GetSpendingAnalyticsForACategoryAsync(catcode, startDate, endDate, direction));
            }
            else
            {
                var allCategories = _dbContext.Categories.Where(c => string.IsNullOrEmpty(c.ParentCode)).ToList();
                foreach(var category in allCategories)
                {
                    analytics.Add(await GetSpendingAnalyticsForACategoryAsync(category.Code, startDate, endDate, direction));
                }
            }

            return analytics;
        }

        private async Task<SpendingAnalytics> GetSpendingAnalyticsForACategoryAsync(string catcode, DateTime? startDate, DateTime? endDate, Direction? direction)
        {
            var listOfCatCodes = new List<string> { catcode };
            listOfCatCodes.AddRange(await GetAllCategoriesByCatCode(catcode));

            var query = _dbContext.Transactions.AsQueryable();
            query = query.Include(t => t.TransactionSplits);
            query = query.Where(t => !string.IsNullOrWhiteSpace(t.CatCode));
            query = query.Where(t => listOfCatCodes.Contains(t.CatCode) || t.TransactionSplits != null && t.TransactionSplits.Where(ts => (listOfCatCodes.Contains(ts.CatCode))).ToList().Count != 0);
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



            var transactions = await query.ToListAsync();
            int count = 0;
            double amount = 0;
            foreach(var transaction in transactions)
            {
                if(listOfCatCodes.Contains(transaction.CatCode))
                {
                    double splitAmount = 0;
                    int splitCount = 0;
                    if (transaction.TransactionSplits != null)
                    {
                        foreach (var transactionSplit in transaction.TransactionSplits)
                        {
                            if (listOfCatCodes.Contains(transactionSplit.CatCode))
                            {
                                splitAmount += transactionSplit.Amount;
                                splitCount++;
                            }
                        }
                    }
                    if (splitAmount == 0 && splitCount == 0)
                    {
                        amount += transaction.Amount;
                        count++;
                    }
                    else
                    {
                        amount += splitAmount;
                        count += splitCount;
                    }

                }
                else if(transaction.TransactionSplits != null)
                {
                    foreach (var transactionSplit in transaction.TransactionSplits)
                    {
                        if (listOfCatCodes.Contains(transactionSplit.CatCode))
                        {
                            amount += transactionSplit.Amount;
                            count++;
                        }
                    }
                }
            }
            return new SpendingAnalytics
            {
                Catcode = catcode,
                Amount = amount,
                Count = count
            };
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
