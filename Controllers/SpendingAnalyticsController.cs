using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models.TransactionModels;
using PersonalFinanceManagement.Services.CategoryServices;
using PersonalFinanceManagement.Services.ErrorServices;

namespace PersonalFinanceManagement.Controllers
{
    [EnableCors("MyCORSPolicy")]
    [ApiController]
    [Route("spending-analytics")]
    public class SpendingAnalyticsController : ControllerBase
    {
        ICategoryService _categoryService;
        private readonly IRequestErrorService _requestErrorService;
        private readonly ILogger<CategoryController> _logger;

        public SpendingAnalyticsController(ILogger<CategoryController> logger, ICategoryService categoryService, IRequestErrorService requestErrorService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _requestErrorService = requestErrorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpendingAnalyticsAsync([FromQuery] string? catcode, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] Direction? direction)
        {
            var groups = await _categoryService.GetSpendingAnalyticsAsync(catcode, startDate, endDate, direction);
            return Ok(new
            {
                groups = groups
            });
        }
    }
}
