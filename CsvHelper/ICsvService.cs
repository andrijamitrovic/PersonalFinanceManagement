namespace PersonalFinanceManagement.CsvHelper
{
    public interface ICsvService
    {
        public IEnumerable<T> ReadCsv<T>(Stream file);
    }
}
