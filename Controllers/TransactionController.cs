using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Services;

namespace PersonalFinanceManagement.Controllers
{
    [EnableCors("MyCORSPolicy")]
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        ITransactionService _transactionsService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionsService = transactionService;
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportTransactions([FromForm] IFormFile file)
        {
            if (file.FileName.Split(".")[1] != "csv")
                return BadRequest("File format must be csv");

            await _transactionsService.ImportTransactions(file);

            return Ok();
        }
    }
}
