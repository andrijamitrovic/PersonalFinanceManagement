using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Services.ErrorServices;
using System.Text;
using PersonalFinanceManagement.Services.CategoryServices;

namespace PersonalFinanceManagement.Controllers
{

    [EnableCors("MyCORSPolicy")]
    [ApiController]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        ICategoryService _categoryService;
        private readonly IRequestErrorService _requestErrorService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService, IRequestErrorService requestErrorService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _requestErrorService = requestErrorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] string? parentId = null)
        {
            var categories = await _categoryService.GetCategoriesAsync(parentId);
            return Ok(categories);
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportCategoriesAsync(IFormFile file)
        {
            List<RequestError> errors = new List<RequestError>();
            var ext = file.FileName.Split(".");

            if (ext.Length < 2 || ext.Last() != "csv")
            {
                errors.Add(_requestErrorService.GetFileNameError());
                return BadRequest(errors);
            }

            var badTransactions = await _categoryService.ImportCategoriesAsync(file);

            return File(Encoding.UTF8.GetBytes(badTransactions), "text/plain", "badCategories.txt");
        }
    }
}
