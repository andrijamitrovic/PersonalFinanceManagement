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

        public async Task<PagedSortedFilteredList<TransactionEntity>> GetTransactionsAsync(List<Kind>? transactionKind, DateOnly? startDate, DateOnly? endDate, int page = 1, int pageSize = 10, SortOrder sortOrder = SortOrder.Asc, string? sortBy = null)
        {

            var query = _dbContext.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "amount":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount);
                        break;
                    case "id":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                    case "beneficiary-name":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.BeneficiaryName) : query.OrderByDescending(x => x.BeneficiaryName);
                        break;
                    case "description":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description);
                        break;
                    case "date":
                        query = sortOrder == SortOrder.Asc ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Amount);
            }
            if (transactionKind?.Count == 0)
            {
                query = query.Where(x => transactionKind.Contains(x.Kind));
            }
            if (startDate != null)
            {
                query = query.Where(x => x.Date >= startDate);
            }
            if (endDate != null)
            {
                query = query.Where(x => x.Date <= endDate);
            }
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount * 1.0 / pageSize);
            if (totalPages == 0)
                totalPages++;

            var transactions = await query.Include(x => x.TransactionSplits).ToListAsync();

            return new PagedSortedFilteredList<TransactionEntity>
            {
                TotalPages = totalPages,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Items = transactions
            };
        }

        public async Task<BussinessProblem?> CategorizeTransactionAsync(string id, string catcode)
        {
            var transactionToUpdate = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Code == catcode);
            if (transactionToUpdate != null)
            {
                if (category != null)
                {
                    transactionToUpdate.CatCode = catcode;
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    return new BussinessProblem
                    {
                        Problem = "category-doesnt-exist",
                        Message = "Category doesn't exist",
                        Details = "The category with this category code doesn't exist."
                    };
                }
            }
            else
            {
                return new BussinessProblem
                {
                    Problem = "transaction-doesnt-exist",
                    Message = "Transaction doesn't exist",
                    Details = "The transaction with this id doesn't exist."
                };
            }
            return null;
        }

        public async Task<BussinessProblem?> SplitTransactionAsync(string id, SplitTransactionCommand splits)
        {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id.Equals(id));
            
            if(transaction == null)
            {
                return new BussinessProblem
                {
                    Problem = "transaction-doesnt-exist",
                    Message = "Transaction doesn't exist",
                    Details = "The transaction with this id doesn't exist."
                };
            }

            var totalAmount = splits.Splits.Sum(s => s.Amount);
            if (totalAmount > transaction.Amount)
            {
                return new BussinessProblem
                {
                    Problem = "split-amount-over-transaction-amount",
                    Message = "Split amount is over the transaction amount",
                    Details = "The sum of amounts of all splits is larger than the transaction amount."
                };
            }
            if (totalAmount < transaction.Amount)
            {
                return new BussinessProblem
                {
                    Problem = "split-amount-under-transaction-amount",
                    Message = "Split amount is under the transaction amount",
                    Details = "The sum of amounts of all splits is smaller than the transaction amount."
                };
            }

            var oldTransactionSplits = _dbContext.TransactionSplits.Where(t => t.TransactionId == transaction.Id).ToList();

            if (oldTransactionSplits.Count() > 0)
            {
                foreach (var split in oldTransactionSplits)
                {
                    _dbContext.TransactionSplits.Remove(split);
                }
            }

            List<TransactionSplitEntity> newTransactionSplits = new List<TransactionSplitEntity>();
            foreach (var split in splits.Splits)
            {
                var newTransactionSplit = new TransactionSplitEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = split.Amount,
                    CatCode = split.Catcode,
                    TransactionId = transaction.Id
                };
                newTransactionSplits.Add(newTransactionSplit);
            }

            await _dbContext.AddRangeAsync(newTransactionSplits);
            await _dbContext.SaveChangesAsync();

            return null;
        }

        public async Task AutoCategorizeTransactionAsync(AutoCategorizeRuleList<Rule> rules)
        {
            foreach (var rule in rules.Rules)
            {
                await _dbContext.Database.ExecuteSqlRawAsync("UPDATE public.transactions SET \"CatCode\" =" + rule.Catcode + " WHERE " + rule.Predicate);
            }
        }
    }
}
