namespace PersonalFinanceManagement.CsvHelper
{
    public interface ICsvService
    {
        public (List<string>,IEnumerable<T>) ReadCsv<T>(Stream stream);
    }
}
