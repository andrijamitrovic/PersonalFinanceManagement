namespace PersonalFinanceManagement.Models.CategoryModels
{
    public class Category
    {
        public string Code { get; set; }
        public string? ParentCode { get; set; }
        public string Name { get; set; }
    }
}
