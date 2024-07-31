using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagement.Models;
using PersonalFinanceManagement.Services.ErrorServices;
using System.Text;
using PersonalFinanceManagement.Services.CategoryServices;
using PersonalFinanceManagement.Models.CategoryModels;

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
        public async Task<IActionResult> GetCategoriesAsync([FromQuery(Name = "parent-id")] string? parentId = null)
        {
            var categories = await _categoryService.GetCategoriesAsync(parentId);
            return Ok(new CategoryList
            {
                Items = categories
            });
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportCategoriesAsync(IFormFile file)
        {
            var badCategories = await _categoryService.ImportCategoriesAsync(file);

            return File(Encoding.UTF8.GetBytes(badCategories), "text/plain", "badCategories.txt");
        }
    }
}
