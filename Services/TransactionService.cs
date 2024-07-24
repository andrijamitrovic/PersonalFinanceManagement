using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using PersonalFinanceManagement.CsvHelper;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Database.Repositories;
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

        public async Task<bool> ImportTransactions(IFormFile file)
        {
            var records = _csvService.ReadCsv<Transaction>(file.OpenReadStream());
            foreach(var record in records)
            {
                var imported = await _transactionRepository.CreateTransaction(_mapper.Map<TransactionEntity>(record));
                if(imported == false)
                {
                    _logger.LogWarning(message: $"[skipped][{record.Id},{record.BeneficiaryName}," +
                                                $"{record.Date},{record.Direction},{record.Amount},{record.Description}," +
                                                $"{record.Currency},{record.Mcc},{record.Kind}]");
                }
            }

            return true;
        }
    }
}
