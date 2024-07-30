using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagement.Models
{
    public class SplitTransactionCommand
    {
        [MaxLength(100)]
        public List<SingleCategorySplit> Splits { get; set; }
    }
}
