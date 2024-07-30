using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Models.TransactionModels;
using PersonalFinanceManagement.Services.ErrorServices;
using PersonalFinanceManagement.Services.TransactionServices;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var badTransactions = await _transactionsService.ImportTransactionsAsync(file);

            return File(Encoding.UTF8.GetBytes(badTransactions), "text/plain", "badTransactions.txt");
        }

        [HttpPost("{id}/categorize")]
        public async Task<IActionResult> CategorizeTransactionAsync([FromRoute] string id, [FromBody] TransactionCategorizeCommand catcode)
        {
            var response = await _transactionsService.CategorizeTransactionAsync(id, catcode.Catcode);
            if(response == null)
            {
                return Ok();
            }
            else
            {
                return StatusCode(440, response);
            }
        }

        [HttpPost("{id}/split")]
        public async Task<IActionResult> SplitTransactionAsync([FromRoute] string id, [FromBody] SplitTransactionCommand splits)
        {
            var response = await _transactionsService.SplitTransactionAsync(id, splits);
            if (response == null)
            {
                return Ok();
            }
            else
            {
                return StatusCode(440, response);
            }
        }
    }
}
