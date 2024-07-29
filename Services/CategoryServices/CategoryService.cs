using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceManagement.CsvHelper;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Database.Repositories.CategoryRepositories;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.CategoryModels;
using PersonalFinanceManagement.Models.TransactionModels;
using System.Collections.Generic;
using System.Globalization;

namespace PersonalFinanceManagement.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        ICategoryRepository _categoryRepository;
        ICsvService _csvService;
        IMapper _mapper;
        ILogger<Category> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ICsvService csvService, IMapper mapper, ILogger<Category> logger)
        {
            _csvService = csvService;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<Category>> GetCategoriesAsync(string? parentCategory)
        {
            var categories = await _categoryRepository.GetCategoriesAsync(parentCategory);
            return categories.Select(c => _mapper.Map<Category>(c)).ToList();
        }

        public async Task<List<SpendingAnalytics>> GetSpendingAnalyticsAsync(string? catcode, DateTime? startDate, DateTime? endDate, Direction? direction)
        {
            return await _categoryRepository.GetSpendingAnalyticsAsync(catcode, startDate, endDate, direction);
        }

        public async Task<string> ImportCategoriesAsync(IFormFile file)
        {
            var (badCategories, categories) = _csvService.ReadCsv<Category>(file.OpenReadStream());

            badCategories.AddRange(await _categoryRepository.ImportCategoriesAsync(categories.Select(i => _mapper.Map<CategoryEntity>(i))));

            return string.Join(Environment.NewLine, badCategories);
        }
    }
}
