
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Transactions;

namespace PersonalFinanceManagement.CsvHelper
{
    public class CsvService : ICsvService
    {
        private readonly ILogger<Transaction> _logger;

        public CsvService(ILogger<Transaction> logger)
        {
            _logger = logger;
        }
        public IEnumerable<T> ReadCsv<T>(Stream stream)
        {
            List<string> badTransactions = new List<string>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Replace("-", "").ToLower(),
                ReadingExceptionOccurred = args =>
                {
                    badTransactions.Add(args.Exception.Message.Split(":").Last().Trim());
                    _logger.LogWarning(message: "[skipped][{row}]",args.Exception.Message.Split(":").Last().Trim());
                    return false;
                }

            };
            var reader = new StreamReader(stream);
            var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<T>();
            return records;
        }
    }
}
