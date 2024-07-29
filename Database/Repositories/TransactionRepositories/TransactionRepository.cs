using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using PersonalFinanceManagement.Database.Entities;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;

namespace PersonalFinanceManagement.Database.Repositories.TransactionRepositories
{
    public class TransactionRepository : ITransactionRepository
    {
        PFMDbContext _dbContext;
        ILogger<TransactionEntity> _logger;

        public TransactionRepository(PFMDbContext dbContext, ILogger<TransactionEntity> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<string>> ImportTransactionsAsync(IEnumerable<TransactionEntity> transactions)
        {
            List<string> badTransactions = new List<string>();
            foreach (var transaction in transactions)
            {

                if (_dbContext.Transactions.Any(t => t.Id == transaction.Id) == false)
                {
                    await _dbContext.AddAsync(transaction);
                }
                else
                {
                    string message = $"[id is already taken][{transaction.Id},{transaction.BeneficiaryName}," +
                                                $"{transaction.Date},{transaction.Direction},{transaction.Amount},{transaction.Description}," +
                                                $"{transaction.Currency},{transaction.Mcc},{transaction.Kind}]";
                    _logger.LogWarning(message);
                    badTransactions.Add(message);
                }
            }
            await _dbContext.SaveChangesAsync();
            return badTransactions;
        }

        public async Task<PagedSortedFilteredList<TransactionEntity>> GetTransactionsAsync(List<Kind>? transactionKind, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 10, SortOrder sortOrder = SortOrder.Asc, string? sortBy = null)
        {

            var query = _dbContext.Transactions.AsQueryable();
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount * 1.0 / pageSize);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "amount":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount);
                        break;
                    case "beneficiary-name":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.BeneficiaryName) : query.OrderByDescending(x => x.BeneficiaryName);
                        break;
                    case "description":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Amount);
            }
            if (transactionKind?.Any() == true)
            {
                query = query.Where(x => transactionKind.Contains(x.Kind));
            }
            if (startDate != null)
            {
                query = query.Where(x => x.Date > startDate);
            }
            if (endDate != null)
            {
                query = query.Where(x => x.Date < endDate);
            }
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var products = await query.ToListAsync();

            return new PagedSortedFilteredList<TransactionEntity>
            {
                TotalPages = totalPages,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Items = products
            };
        }

        public async Task<string> CategorizeTransactionAsync(string id, string catcode)
        {
            var transactionToUpdate = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Code == catcode);
            if (transactionToUpdate != null && category != null)
            {
                transactionToUpdate.CatCode = catcode;
                await _dbContext.SaveChangesAsync();
            }
            return "123";
        }

        public async Task SplitTransactionAsync(string id, List<Split> splits)
        {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id.Equals(id));
            
            if(transaction == null)
            {
                return;
            }

            if(transaction.Splits != null && transaction.Splits.Count > 0)
            {
                foreach (var split in transaction.Splits)
                {
                    _dbContext.Transactions.Remove(split);
                }
            }

            List<TransactionEntity> newTransactions = new List<TransactionEntity>();
            foreach (Split split in splits)
            {
                var newTransaction = new TransactionEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    BeneficiaryName = transaction.BeneficiaryName,
                    Date = transaction.Date,
                    Direction = transaction.Direction,
                    Description = transaction.Description,
                    Currency = transaction.Currency,
                    Mcc = transaction.Mcc,
                    Kind = transaction.Kind,
                    SplitId = transaction.Id,
                    Amount = split.Amount,
                    CatCode = split.Catcode
                };
                newTransactions.Add(newTransaction);
            }

            await _dbContext.AddRangeAsync(newTransactions);
            await _dbContext.SaveChangesAsync();

        }
    }
}
