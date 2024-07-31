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
        public async Task<IActionResult> GetTransactionsAsync([FromQuery(Name = "transaction-kind")] string? transactionKind, [FromQuery(Name = "start-date")] string? startDate, [FromQuery(Name = "end-date")] string? endDate, [FromQuery] int page = 1, [FromQuery(Name = "page-size")] int pageSize = 10, [FromQuery(Name = "sort-order")] SortOrder sortOrder = SortOrder.Asc, [FromQuery(Name = "sort-by")] string? sortBy = null)
        {
            List<Kind>? _transactionKind = null;
            if (transactionKind != null)
            {
                _transactionKind = new List<Kind>();
                foreach (var kind in transactionKind.Split(','))
                {
                    _transactionKind.Add(Enum.Parse<Kind>(kind, ignoreCase: true));
                }
            }
            DateOnly? _startDate = null;
            if (startDate != null)
            {
                _startDate = DateOnly.ParseExact(startDate, "M/d/yyyy");
            }
            DateOnly? _endDate = null;
            if (endDate != null)
            {
                _endDate = DateOnly.ParseExact(endDate, "M/d/yyyy");
            }

            var transactions = await _transactionsService.GetTransactionsAsync(_transactionKind, _startDate, _endDate, page, pageSize, sortOrder, sortBy);
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

        [HttpPost("auto-categorize")]
        public async Task<IActionResult> AutoCategorizeTransactionAsync()
        {
            await _transactionsService.AutoCategorizeTransactionAsync();
            return Ok();
        }
    }
}
