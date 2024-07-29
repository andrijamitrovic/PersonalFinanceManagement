using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagement.CsvHelper;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Database.Repositories.TransactionRepositories;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;
using System.Globalization;

namespace PersonalFinanceManagement.Services.TransactionServices
{
    public class TransactionService : ITransactionService
    {
        ITransactionRepository _transactionRepository;
        ICsvService _csvService;
        IMapper _mapper;
        ILogger<Transaction> _logger;

        public TransactionService(ITransactionRepository transactionRepository, ICsvService csvService, IMapper mapper, ILogger<Transaction> logger)
        {
            _csvService = csvService;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CategorizeTransactionAsync(string id, string catcode)
        {
            return await _transactionRepository.CategorizeTransactionAsync(id, catcode);
        }

        public async Task<PagedSortedFilteredList<Transaction>> GetTransactionsAsync(List<Kind>? transactionKind, DateTime? startDate, DateTime? endDate, int page, int pageSize, SortOrder sortOrder, string? sortBy)
        {
            var transactions = await _transactionRepository.GetTransactionsAsync(transactionKind, startDate, endDate, page, pageSize, sortOrder, sortBy);
            return _mapper.Map<PagedSortedFilteredList<Transaction>>(transactions);
        }

        public async Task<string> ImportTransactionsAsync(IFormFile file)
        {
            var (badTransactions, transactions) = _csvService.ReadCsv<Transaction>(file.OpenReadStream());

            var groupedTransactions = transactions.GroupBy(t => t.Id);

            var uniqueTransactions = new List<Transaction>();
            var duplicateTransactions = new List<Transaction>();

            foreach (var group in groupedTransactions)
            {
                if (group.Count() == 1)
                {
                    uniqueTransactions.Add(group.First());
                }
                else
                {
                    uniqueTransactions.Add(group.First());
                    duplicateTransactions.AddRange(group.Skip(1));
                }
            }
            badTransactions.AddRange(duplicateTransactions.Select(t => $"[id repeated in the file][{t.Id},{t.BeneficiaryName}," +
                                                                       $"{t.Date},{t.Direction},{t.Amount},{t.Description},{t.Currency}," +
                                                                       $"{t.Mcc},{t.Kind},{t.CatCode}]"));
            badTransactions.AddRange(await _transactionRepository.ImportTransactionsAsync(uniqueTransactions.Select(i => _mapper.Map<TransactionEntity>(i))));

            return string.Join(Environment.NewLine, badTransactions);
        }
        public async Task SplitTransactionAsync(string id, List<Split> splits)
        {
            await _transactionRepository.SplitTransactionAsync(id, splits);

        }
    }
}
