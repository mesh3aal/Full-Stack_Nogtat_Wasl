
using Application_Layer.Services.IdentityAndAccess;

namespace Application_Layer.Services.CatalogAndClassifications;

public class SubCategoryService(ApplicationDbContext context, ICurrentUserService userService) : ISubCategoryService
{
    public async Task<Result<List<CategorySubCategoryResponse>>> GetAllCategoriesWithSubCategories(int pageNumber = 1, int pageSize = 10)
    {
        var result = await context.Categories.AsNoTracking()
            .Include(x => x.SubCategories)
            .Select(o => new CategorySubCategoryResponse
            {
                CategoryId = o.CategoryId,
                CategoryName = o.CategoryName,
                SubCategories = o.SubCategories.Select(x => new SubCategoryResponse
                {
                    SubCategoryId = x.SubCategoryId,
                    SubCategoryName = x.Name
                }).ToList()
            }).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsSplitQuery().ToListAsync();
        return Result<List<CategorySubCategoryResponse>>.Success(result);
    }

    public async Task<Result<List<SubCategoryResponse>>> GetSubCategories(int categoryId, int pageNumber=1, int pageSize=10)
    {
        var subCategories = await context.SubCategories
            .Include(x => x.Category)
            .Where(x => x.CategoryId == categoryId)
            .Select(x => new SubCategoryResponse
            {
                SubCategoryId =  x.SubCategoryId,
                SubCategoryName = x.Name
            }).AsNoTracking().AsSplitQuery().Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .ToListAsync();
        return  Result<List<SubCategoryResponse>>.Success(subCategories);
    }

    public async Task<Result<AddSubCategoryRequest>> AddSubCategory(AddSubCategoryRequest request)
    {
        var currentUserRole = userService.GetUserRole();
        if (!userService.IsAuthenticatedAsync() || currentUserRole[0] != "Admin")
        {
            return Result.Unauthorized("غير مصرح لك بالوصول");
        }
        var category = await context.Categories.FindAsync(request.CategoryId);
        if (category == null)
            Result<AddSubCategoryRequest>.NotFound("لم يتم إجاد الصنق المطلوب");
        
        var subCategoryExists = await context.SubCategories
            .AnyAsync(x => x.Name.Trim().ToLower() == request.SubCategoryName.Trim().ToLower());

        if (subCategoryExists) 
        {
            return Result.Conflict("هذا فرع الصنف موجود بالفعل");
        }

        var newSubCategory = new SubCategory
        {
            CategoryId = request.CategoryId,
            Name = request.SubCategoryName,
        };

        context.SubCategories.Add(newSubCategory);

        var saveResult = await context.SaveChangesAsync() > 0;
        if (!saveResult)
        {
            return Result.Error("حدث خطاء اثناء الحفظ");
        }

        return Result.Success(request);
    }

    public async Task<Result<UpdateSubCategoryRequest>> UpdateSubCategory(int subCategoryId, int categoryId, string? newName)
    {
        var currentUserRole = userService.GetUserRole();
        if (currentUserRole == null || currentUserRole.Count <= 0)
        {
            return Result.Unauthorized();
        }
        if (!userService.IsAuthenticatedAsync() || currentUserRole[0] != "Admin")
        {
            return Result.Unauthorized("غير مصرح لك بالوصول");
        }
        
        var subcategory = await context.SubCategories.FindAsync(subCategoryId);
        if (subcategory == null)
        {
            return Result.NotFound("لم يتم إجاد فرع الصنف المطلوب");
        }

        var categoryExists = await context.Categories.AnyAsync(i => i.CategoryId == categoryId);
        if (!categoryExists)
        {
            return Result.NotFound("لم يتم إجاد الصنف المطلوب");
        }
        
        if (!string.IsNullOrWhiteSpace(newName))
        {
            subcategory.Name = newName;
        }
        subcategory.CategoryId = categoryId;

        await context.SaveChangesAsync();

        return Result.Success(new UpdateSubCategoryRequest
        {
            CategoryId = categoryId,
            SubCategoryName = subcategory.Name 
        });
    }

    public async Task<Result> DeleteSubCategory(int subCategoryId)
    {
        var currentUserRole = userService.GetUserRole();
        if (!userService.IsAuthenticatedAsync() || currentUserRole![0] != "Admin")
        {
            return Result.Unauthorized("غير مصرح لك بالوصول");
        }
        
        var subcategory = await context.SubCategories.FindAsync(subCategoryId);
        if (subcategory == null)
            return Result.NotFound("لم يتم إجاد فرع الصنف المطلوب");
        context.SubCategories.Remove(subcategory);
        if (await context.SaveChangesAsync() <= 0)
            return Result.Error("حدث خطاء اثناء الحفظ");
        await context.SaveChangesAsync();
        return Result.Success();
    }
}
