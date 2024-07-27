using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using PersonalFinanceManagement.CsvHelper;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Database.Repositories;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;
using System.Globalization;

namespace PersonalFinanceManagement.Services
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

        public async Task<PagedSortedFilteredList<Transaction>> GetTransactionsAsync(List<Kind>? transactionKind, DateTime? startDate, DateTime? endDate, int page, int pageSize, SortOrder sortOrder, string? sortBy)
        {
            var transactions = await _transactionRepository.GetTransactionsAsync(transactionKind, startDate, endDate, page, pageSize, sortOrder, sortBy);
            return _mapper.Map<PagedSortedFilteredList<Transaction>>(transactions);
        }

        public async Task<string> ImportTransactions(IFormFile file)
        {
            var (badTransactions, transactions) = _csvService.ReadCsv<Transaction>(file.OpenReadStream());
            var grouped = transactions
            .GroupBy(t => t.Id);

            var firstTransactions = grouped
                .Select(g => g.First());

            var duplicateTransactions = grouped
                .SelectMany(g => g.Skip(1))
                .Select(g => $"[there is a row with this id in the file][{g.Id},{g.BeneficiaryName}," + 
                                $"{g.Date},{g.Direction},{g.Amount},{g.Description}," +
                                $"{g.Currency},{g.Mcc},{g.Kind}]");

            badTransactions.AddRange(duplicateTransactions);
            badTransactions.AddRange(await _transactionRepository.CreateTransactions(firstTransactions.Select(i => _mapper.Map<TransactionEntity>(i))));

            return string.Join(Environment.NewLine, badTransactions);
        }
    }
}
