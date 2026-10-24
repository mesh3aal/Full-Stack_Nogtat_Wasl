using Application_Layer.Models.RequestDTO.CategoryController;
using Application_Layer.Models.SendDTO.CategoryController;
using Application_Layer.Services.CatalogAndClassifications;

namespace WebAPI.Controllers.Admin
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //[Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.Categories.GetAll)]
        public async Task<Result<CategoryListResponse>> GetCategories([FromQuery] string? search)
        {
            return await _categoryService.GetCategoryAllAsync(search);
        }


        ////[Authorize(Roles = "Admin")]
        //[TranslateResultToActionResult]
        //[HttpGet(Routing.Categories.GetCategory)]
        //public async Task<Result<SendCategoriesDto>> GetOnlyCategories()
        //{
        //    return await _categoryService.GetOnlyCategory();

        //}


        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.Categories.GetById)]
        public async Task<Result<CategoryResponse>> GetCategory(int id)
        {
            return await _categoryService.GetCategory(id);
        }

        [TranslateResultToActionResult]
        [Authorize(Roles = "Admin")]
        [HttpPost(Routing.Categories.Create)]
        public async Task<Result<CreateCategoryDto>> Create([FromBody] CreateCategoryDto dto)
        {
            return await _categoryService.CreateCategory(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut(Routing.Categories.Update)]
        [TranslateResultToActionResult]
        public async Task<Result<UpdateCategoryDto>> Update([FromBody] UpdateCategoryDto dto)
        {
            return await _categoryService.UpdateCategory(dto);
        }

        [TranslateResultToActionResult]
        [Authorize(Roles = "Admin")]
        [HttpDelete(Routing.Categories.Delete)]
        public async Task<Result<string>> Delete(int id)
        {
            return await _categoryService.DeleteCategory(id);
        }
    }
}