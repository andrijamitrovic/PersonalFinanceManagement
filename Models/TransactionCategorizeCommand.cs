using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagement.Models
{
    public class TransactionCategorizeCommand
    {
        [MaxLength(64)]
        public string Catcode { get; set; }
    }
}
