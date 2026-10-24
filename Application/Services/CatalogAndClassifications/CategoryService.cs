using Application_Layer.Models.RequestDTO.CategoryController;
using Application_Layer.Models.SendDTO.CategoryController;

namespace Application_Layer.Services.CatalogAndClassifications
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> CategoryFilter(IQueryable<Category> query, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            return query.Where(x => EF.Functions.Like(x.CategoryName, $"%{search}%"));
        }

        public async Task<Result<CategoryListResponse>> GetCategoryAllAsync(string? search)
        {
            IQueryable<Category> query = _context.Categories
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = CategoryFilter(query, search);
            }

            var categories = await query
                .Select(x => new CategoryResponse
                {
                    id = x.CategoryId,
                    CategoryIcon = x.CategoryIcon,
                    CategoryName = x.CategoryName,
                    Source = x.Source,
                    SubCategory = x.SubCategories.Select(s => new SubCategoryResponse
                    {
                        SubCategoryName = s.Name,
                        SubCategoryIcon=s.supCategoryIcon,
                        SubCategoryId = s.SubCategoryId
                    }).ToList()
                })
                .ToListAsync();

            if (!categories.Any())
            {
                return Result<CategoryListResponse>.NotFound("لم يتم العثور على الفئة");
            }

            return Result<CategoryListResponse>.Success(
                new CategoryListResponse
                {
                    categories = categories,
                    totalCount = categories.Count
                },
                "تم جلب البيانات بنجاح"
            );
        }
        public async Task<Result<SendCategoriesDto>> GetOnlyCategory()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(x => new CategoryDto
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                })
                .ToListAsync();

            var result = new SendCategoriesDto
            {
                categories = categories
            };

            return Result<SendCategoriesDto>
                .Success(result, "تم جلب البيانات بنجاح");
        }

        public async Task<Result<CreateCategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            if (dto == null)
                return Result<CreateCategoryDto>.Invalid(new ValidationError("البيانات فارغة"));

            if (await _context.Categories.AnyAsync(x => x.CategoryName == dto.CategoryName))
            {
                return Result<CreateCategoryDto>.Conflict("الفئة موجودة مسبقاً");
            }

            var mainEntity = CreateCategoryDto.ToEntity(dto);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Categories.AddAsync(mainEntity);
                await _context.SaveChangesAsync();

                if (dto.SupCategory != null && dto.SupCategory.Any())
                {
                    var subEntities = dto.SupCategory.Select(subDto =>
                    {
                        var subEntity = SupCategoryCreate.ToEntity(subDto);
                        subEntity.CategoryId = mainEntity.CategoryId; 
                        return subEntity;
                    }).ToList();

                    await _context.SubCategories.AddRangeAsync(subEntities);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Result<CreateCategoryDto>.Success(dto, "تمت الإضافة بنجاح");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<CreateCategoryDto>.Error($"حدث خطأ أثناء الحفظ: {ex.Message}");
            }
        }
        public async Task<Result<UpdateCategoryDto>> UpdateCategory(UpdateCategoryDto dto)
        {
            var category = await _context.Categories
                .Include(x => x.SubCategories)
                .FirstOrDefaultAsync(x => x.CategoryId == dto.Id);

            if (category == null)
                return Result<UpdateCategoryDto>.NotFound("لم يتم العثور على الفئة");

            category.CategoryName = dto.CategoryName;
            category.CategoryIcon = dto.CategoryIcon;
            if (!string.IsNullOrEmpty(dto.Source))
            {
                category.Source = dto.Source;
            }
            if (dto.SupCategory != null)
            {
                var incomingIds = dto.SupCategory.Select(s => s.SubCategoryName).ToList();
                var subsToRemove = category.SubCategories
                    .Where(oldSub => !incomingIds.Contains(oldSub.Name))
                    .ToList();

                _context.SubCategories.RemoveRange(subsToRemove);

                // ب - إضافة أو تحديث العناصر
                foreach (var subDto in dto.SupCategory)
                {
                    var existingSub = category.SubCategories
                        .FirstOrDefault(s => s.Name == subDto.SubCategoryName && s.SubCategoryId != 0);

                    if (existingSub != null)
                    {
                        SupCategoryUpdatedto.Update(existingSub, subDto);
                    }
                    else
                    {
                        var newSub = SupCategoryUpdatedto.ToEntity(subDto);
                        newSub.CategoryId = category.CategoryId; // ربطه بالأب
                        category.SubCategories.Add(newSub);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Result<UpdateCategoryDto>.Success(dto, "تم التعديل بنجاح");
            }
            catch (Exception ex)
            {
                return Result<UpdateCategoryDto>.Error($"خطأ أثناء التحديث: {ex.Message}");
            }
        }
        
        public async Task<Result<string>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Result<string>.Success("تم الحذف بنجاح");
            }
            else
            {
                return Result<string>.NotFound("لم يتم العثور على الفئة");
            }
        }

        public async Task<Result<CategoryResponse>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return Result<CategoryResponse>.NotFound("لم يتم العثور على الفئة");
            }

            var response = new CategoryResponse
            {
                id = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryIcon = category.CategoryIcon,
                Source = category.Source,
                SubCategory = category.SubCategories.Select(x => new SubCategoryResponse
                {
                    SubCategoryId = x.SubCategoryId,
                    SubCategoryName = x.Name,
                }).ToList()
            };

            return Result<CategoryResponse>.Success(response, "تم العثور على الفئة");
        }
    }
}
