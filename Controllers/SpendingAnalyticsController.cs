using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models;
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
        public async Task<IActionResult> GetSpendingAnalyticsAsync([FromQuery] string? catcode, [FromQuery(Name = "start-date")] string? startDate, [FromQuery(Name = "end-date")] string? endDate, [FromQuery] string? direction)
        {
            Direction? _direction = null;
            if (direction != null)
            {
                _direction = Enum.Parse<Direction>(direction, ignoreCase: true);
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
            var groups = await _categoryService.GetSpendingAnalyticsAsync(catcode, _startDate, _endDate, _direction);
            return Ok(new SpendingByCategory
            {
                Groups = groups
            });
        }
    }
}
