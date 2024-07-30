using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagement.Models
{
    public class SingleCategorySplit
    {
        [MaxLength(64)]
        public string Catcode { get; set; }
        public double Amount { get; set; }
    }
}
