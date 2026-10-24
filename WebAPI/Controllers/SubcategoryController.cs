
using Application_Layer.Services.CatalogAndClassifications;

namespace WebAPI.Controllers;

[ApiController]
public class SubcategoryController(ISubCategoryService service) : ControllerBase
{
    [TranslateResultToActionResult]
    [HttpGet(Routing.Subcategories.GetAllWithCategories)]
    public async Task<Result<List<CategorySubCategoryResponse>>> GetAllCategoriesWithSubCategories(int pageNumber = 1, int pageSize = 10)
    {
        return await service.GetAllCategoriesWithSubCategories(pageNumber, pageSize);
    }
    [TranslateResultToActionResult]
    [HttpGet(Routing.Subcategories.GetByCategoryId)]
    public async Task<Result<List<SubCategoryResponse>>> GetSubCategories(int categoryId,  int pageNumber = 1, int pageSize = 10)
    { 
        return await service.GetSubCategories(categoryId,  pageNumber, pageSize);  
    }
    [TranslateResultToActionResult]
    [HttpPost(Routing.Subcategories.Create)]
    public async Task<Result<AddSubCategoryRequest>> AddSubCategory(AddSubCategoryRequest request)
    {
        return await service.AddSubCategory(request);
    }
    [TranslateResultToActionResult]
    [HttpPatch(Routing.Subcategories.Update)]
    public async Task<Result<UpdateSubCategoryRequest>> UpdateSubcategory(int subCategoryId,int categoryId, string? newName)
    {
        return await service.UpdateSubCategory(subCategoryId, categoryId, newName);
    }
    [TranslateResultToActionResult]
    [HttpDelete(Routing.Subcategories.Delete)]
    public async Task<Result> DeleteSubcategory(int subCategoryId)
    {
        return await service.DeleteSubCategory(subCategoryId);
    }
}
