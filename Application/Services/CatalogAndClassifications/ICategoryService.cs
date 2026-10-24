
using Application_Layer.Models.RequestDTO.CategoryController;
using Application_Layer.Models.SendDTO.CategoryController;

namespace Application_Layer.Services.CatalogAndClassifications
{
    public interface ICategoryService
    {
        Task<Result<CategoryListResponse>> GetCategoryAllAsync(string? search);
        Task<Result<CategoryResponse>> GetCategory(int id);
        Task<Result<string>> DeleteCategory(int id);
        Task<Result<UpdateCategoryDto>> UpdateCategory( UpdateCategoryDto dto);
        Task<Result<CreateCategoryDto>> CreateCategory(CreateCategoryDto dto);
        Task<Result<SendCategoriesDto>> GetOnlyCategory();





    }
}
