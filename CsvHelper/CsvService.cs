﻿
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using PersonalFinanceManagement.Models.TransactionModels;
using System.Globalization;

namespace PersonalFinanceManagement.CsvHelper
{
    public class CsvService : ICsvService
    {
        private readonly ILogger<Transaction> _logger;

        public CsvService(ILogger<Transaction> logger)
        {
            _logger = logger;
        }
        public (List<string>, IEnumerable<T>) ReadCsv<T>(Stream stream)
        {
            List<string> badRecords = new List<string>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = x =>
                {
                    if (x.InvalidHeaders.Length > 0)
                        foreach (var invalidHeader in x.InvalidHeaders)
                            foreach(var header in invalidHeader.Names)
                                badRecords.Add($"[header is missing][{header}]");
                },
                PrepareHeaderForMatch = args => args.Header.Replace("-", "").ToLower(),
                ReadingExceptionOccurred = args =>
                {
                    badRecords.Add($"[bad row][{args.Exception.Message.Split(":").Last().Trim()}]");
                    _logger.LogWarning(message: $"[bad row][{args.Exception.Message.Split(":").Last().Trim()}]");
                    return false;
                }
                
            };
            var reader = new StreamReader(stream);
            var csv = new CsvReader(reader, config);
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            var records = csv.GetRecords<T>();
            return (badRecords, records);
        }
    }
}
