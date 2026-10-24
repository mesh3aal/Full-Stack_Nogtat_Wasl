
namespace Application_Layer.Services.CatalogAndClassifications;

public interface ISubCategoryService
{
    Task<Result<List<CategorySubCategoryResponse>>> GetAllCategoriesWithSubCategories(int pageNumber, int pageSize);
    Task<Result<List<SubCategoryResponse>>> GetSubCategories(int categoryId, int pageNumber, int pageSize );
    Task<Result<AddSubCategoryRequest>>  AddSubCategory(AddSubCategoryRequest request);
    Task<Result<UpdateSubCategoryRequest>> UpdateSubCategory(int subCategoryId, int categoryId, string? newName);
    Task<Result> DeleteSubCategory(int subCategoryId);
}
