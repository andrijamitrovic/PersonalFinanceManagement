using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;
using PersonalFinanceManagement.Services.ErrorServices;
using PersonalFinanceManagement.Services.TransactionServices;
using System.Text;

namespace PersonalFinanceManagement.Controllers
{
    [EnableCors("MyCORSPolicy")]
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        ITransactionService _transactionsService;
        private readonly IRequestErrorService _requestErrorService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService, IRequestErrorService requestErrorService)
        {
            _logger = logger;
            _transactionsService = transactionService;
            _requestErrorService = requestErrorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsAsync([FromQuery] List<Kind>? transactionKind, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] SortOrder sortOrder = SortOrder.Asc, [FromQuery] string? sortBy = null)
        {
            var transactions = await _transactionsService.GetTransactionsAsync(transactionKind, startDate, endDate, page, pageSize, sortOrder, sortBy);
            return Ok(transactions);
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportTransactionsAsync(IFormFile file)
        {
            List<RequestError> errors = new List<RequestError>();
            var ext = file.FileName.Split(".");

            if (ext.Length < 2 || ext.Last() != "csv")
            {
                errors.Add(_requestErrorService.GetFileNameError());
                return BadRequest(errors);
            }

            var badTransactions = await _transactionsService.ImportTransactionsAsync(file);

            return File(Encoding.UTF8.GetBytes(badTransactions), "text/plain", "badTransactions.txt");
        }

        [HttpPost("{id}/categorize")]
        public async Task<IActionResult> CategorizeTransactionAsync([FromRoute] string id, [FromBody] TransactionCategorizeCommand catcode)
        {
            await _transactionsService.CategorizeTransactionAsync(id, catcode.Catcode);
            return Ok();
        }

        [HttpPost("{id}/split")]
        public async Task<IActionResult> SplitTransactionAsync([FromRoute] string id, [FromBody] SplitTransactionCommand splits)
        {
            await _transactionsService.SplitTransactionAsync(id, splits);
            return Ok();
        }
    }
}
