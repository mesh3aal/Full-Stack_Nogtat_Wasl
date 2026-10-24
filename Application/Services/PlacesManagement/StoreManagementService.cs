using Application_Layer.Models.RequestDTO.StoreController;
using Application_Layer.Services.IdentityAndAccess;
using Application_Layer.Services.InfrastructureAndUtilities;

namespace Application_Layer.Services.PlacesManagement
{
    public class StoreManagementService(ApplicationDbContext context, IWebHostEnvironment env, IUrlProvider urlProvider, ICurrentUserService currentUser)
        : IStoreManagementService
    {
        public async Task<Result<PaginatedResult<StoreResponsDto>>> GetStoreAllAsync(StoreFilterDto filterDto)
        {
            var query = context.Places
                .Include(x => x.User)
                .Include(x => x.SubCategory)
                .ThenInclude(sc => sc.Category)
                .Include(x => x.Images)
                .AsNoTracking();


            if (!string.IsNullOrWhiteSpace(filterDto.Search))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.PlaceName, $"%{filterDto.Search}%"));
            }

            if (filterDto.Status.HasValue)
            {
                query = query.Where(x => x.State == filterDto.Status);
            }

            var totalCount = await query.CountAsync();


            var page = filterDto.Page <= 0 ? 1 : filterDto.Page;
            var pageSize = filterDto.PageSize <= 0 ? 10 : filterDto.PageSize;


            var stores = await query
                .OrderByDescending(x => x.PlaceId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StoreResponsDto
                {
                    Id = x.PlaceId,
                    Name = x.PlaceName == null ? "unknow" : x.PlaceName,
                    OwnerName = x.User != null ? x.User.UserName : "",
                    Category = x.SubCategory.Category.CategoryName.ToString(),

                    ImageUrl =
                        x.Images
                            .OrderByDescending(i => i.PlaceImageId)
                            .Select(i => i.ImageUrl)
                            .Take(1)
                            .FirstOrDefault(),
                    Status = x.State == PlaceStatus.Active ? "نشط" :
                        x.State == PlaceStatus.Pending ? "قيد المراجعة" :
                        x.State == PlaceStatus.Rejected ? "مرفوض" :
                        "مغلق"
                })
                .ToListAsync();

            var pagination = PaginatedResult<StoreResponsDto>
                .Sucess(stores, totalCount, page, pageSize);

            
            return Result<PaginatedResult<StoreResponsDto>>
                .Success(pagination, "تم جلب بيانات المحلات");
        }


        public async Task<Result<StoreStatusTotalPlaseDto>> GetStatusTotalPlase()
        {
            var result = await context.Places
                .AsNoTracking()
                .GroupBy(x => 1)
                .Select(g => new StoreStatusTotalPlaseDto
                {
                    Total = g.Count(),
                    Active = g.Count(x => x.State == PlaceStatus.Active),
                    Pending = g.Count(x => x.State == PlaceStatus.Pending),
                    Rejected = g.Count(x => x.State == PlaceStatus.Rejected)
                })
                .FirstAsync();
            return Result<StoreStatusTotalPlaseDto>.Success(result, "تم جلب إجمالي عدد المحلات");
        }


        
        
        public async Task<Result<SuspendAndActivateStoreDto>> ClosedStore(int placeId)
        {
            var place = await context.Places.FirstOrDefaultAsync(p => p.PlaceId == placeId);

            if (place == null)
            {
                return Result<SuspendAndActivateStoreDto>.NotFound("المحل غير موجود");
            }
            if (place.State == PlaceStatus.Closed)
            {
                return Result<SuspendAndActivateStoreDto>.Conflict( "هذا المحل مغلق بالفعل");
            }

            place.State = PlaceStatus.Closed;
            try
            {
                await context.SaveChangesAsync();

                return Result<SuspendAndActivateStoreDto>.Success(new SuspendAndActivateStoreDto
                {
                    PlaceID = place.PlaceId
                }, "تم اغلاقه المحل بنجاح");
            }
            catch (Exception ex)
            {
                return Result<SuspendAndActivateStoreDto>.Error("حدث خطأ أثناء تحديث حالة المحل");
            }
        }



       
        public async Task<Result<SuspendAndActivateStoreDto>> RejectedStore(int placeId)
        {
            var place = await context.Places.FirstOrDefaultAsync(p => p.PlaceId == placeId);

            if (place == null)
            {
                return Result<SuspendAndActivateStoreDto>.NotFound("المحل غير موجود");
            }
            if (place.State == PlaceStatus.Rejected)
            {
                return Result<SuspendAndActivateStoreDto>.Conflict("هذا المحل مرفوض بالفعل");
            }

            place.State = PlaceStatus.Rejected;
            try
            {
                await context.SaveChangesAsync();

                return Result<SuspendAndActivateStoreDto>.Success(new SuspendAndActivateStoreDto
                {
                    PlaceID = place.PlaceId
                }, "تم رفضة المحل بنجاح");
            }
            catch (Exception ex)
            {
                return Result<SuspendAndActivateStoreDto>.Error("حدث خطأ أثناء تحديث حالة المحل");
            }
        }


        public async Task<Result<SuspendAndActivateStoreDto>> ActivateStore(int placeID)
        {
            var place = await context.Places.FirstOrDefaultAsync(p => p.PlaceId == placeID);
            if (place == null)
            {
                return Result<SuspendAndActivateStoreDto>.NotFound( "المحل غير موجود");
            }

            if (place.State == PlaceStatus.Active)
            {
                return Result<SuspendAndActivateStoreDto>.NotFound("المحل نشط بالفعل");
            }

            place.State = PlaceStatus.Active;

            try
            {
                await context.SaveChangesAsync();

                return Result<SuspendAndActivateStoreDto>.Success(new SuspendAndActivateStoreDto
                {
                    PlaceID = place.PlaceId
                }, "تم تنشيط المحل بنجاح");
            }
            catch (Exception)
            {
                return Result<SuspendAndActivateStoreDto>.Error( "حدث خطأ أثناء محاولة التنشيط");
            }
        }


        
        public async Task<Result<string>> ChangeStoreStatus(int id, PlaceStatus status)
        {
            var place = await context.Places.FindAsync(id);

            if (place == null)
                return Result<string>.NotFound("المحل غير موجود");

            if (place.State == status)
                return Result<string>.Conflict("الحالة نفسها بالفعل");

            place.State = status;
            await context.SaveChangesAsync();

            return Result<string>.Success("تم تحديث الحالة");
        }

        public async Task<Result<string>> DeleteStore(int placeId)
        {
            var place = await context.Places.FindAsync(placeId);

            if (place == null)
            {
                return Result<string>.NotFound( "المحل غير موجود بالفعل");
            }
            try
            {
                context.Places.Remove(place);
                await context.SaveChangesAsync();

                return Result<string>.Success("تم حذف المحل بنجاح");
            }
            catch (Exception)
            {
                return Result<string>.Error("فشل حذف المحل بسبب قيود في قاعدة البيانات");
            }
        }
        public async Task<Result<EditStore>> EditingForStore(EditStore model)
        {
            if (!currentUser.IsAuthenticatedAsync()) return Result<EditStore>.Unauthorized();

            var userRole = currentUser.GetUserRole()[0].ToLower();

            // Users are completely forbidden from this endpoint
            if (userRole == "user") return Result<EditStore>.Forbidden();

            var isAdmin = userRole == "admin";
            var isOwner = userRole == "owner";

            var place = await context.Places
                .Include(x => x.CommericalRegisteration)
                .Include(x => x.Location)
                .Include(x => x.Images)
                .Include(x => x.Phones)
                .FirstOrDefaultAsync(p => p.PlaceId == model.PlaceId);

            if (place == null)
                return Result<EditStore>.NotFound("المحل غير موجود");

            // Verify ownership for Owner role
            if (isOwner && place.UserId != currentUser.GetUserId())
                return Result<EditStore>.Forbidden();

            // --- Basic fields (Admin + Owner) ---
            if (!string.IsNullOrEmpty(model.Name))
                place.PlaceName = model.Name;

            if (!string.IsNullOrEmpty(model.Description))
                place.Description = model.Description;

            if (model.OpeningTime.HasValue)
                place.OpeningTime = model.OpeningTime;

            if (model.ClosingTime.HasValue)
                place.ClosingTime = model.ClosingTime;

            // --- Location fields (Admin + Owner) ---
            if (model.DirectorateId.HasValue && model.DirectorateId.Value != place.Location.DirectorateId)
            {
                var exists = await context.Directorates.AnyAsync(d => d.Id == model.DirectorateId.Value);
                if (!exists) return Result<EditStore>.Error("المديرية المختارة غير موجودة");
                place.Location.DirectorateId = model.DirectorateId.Value;
            }

            if (model.DistrictId.HasValue && model.DistrictId.Value != place.Location.DistrictId)
            {
                var exists = await context.Districts.AnyAsync(d => d.Id == model.DistrictId.Value);
                if (!exists) return Result<EditStore>.Error("المنطقة المختارة غير موجودة");
                place.Location.DistrictId = model.DistrictId.Value;
            }
            // Subcategory (Admin + Owner )
            if (model.SubCategoryId.HasValue && model.SubCategoryId.Value != place.SubCategoryId)
            {
                var subCategoryExists = await context.SubCategories.AnyAsync(sc => sc.SubCategoryId == model.SubCategoryId.Value);
                if (!subCategoryExists)
                    return Result<EditStore>.Invalid(new ValidationError("التصنيف الفرعي المختار غير موجود"));
                place.SubCategoryId = model.SubCategoryId.Value;
            }

            if (!string.IsNullOrEmpty(model.NearestLandmark))
                place.Location.NearestLandmark = model.NearestLandmark;

            if (model.Latitude.HasValue)
                place.Location.Latitude = model.Latitude.Value;

            if (model.Longitude.HasValue)
                place.Location.Longitude = model.Longitude.Value;
            
            // --- Phone numbers (Admin + Owner) ---
            if (model.PhoneNumbers != null)
            {
                place.Phones.Clear();
                foreach (var phone in model.PhoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    place.Phones.Add(new PhoneNumber { Number = phone, PlaceId = place.PlaceId });
                }
            }

            if (model.StoreImages != null && model.StoreImages.Count > 0)
            {
                // Use place.PlaceId consistently
                string uploadsFolder = Path.Combine(env.WebRootPath, $"images/stores/{place.PlaceId}/additional");
    
                if (Directory.Exists(uploadsFolder))
                {
                    // 1. Clear files inside the directory safely
                    foreach (string filePath in Directory.EnumerateFiles(uploadsFolder))
                    {
                        File.Delete(filePath);
                    }
        
                    // 2. Clear from database (Matching place.PlaceId to ensure consistency)
                    var placeImages = context.PlaceImages.Where(x => x.PlaceId == place.PlaceId);
                    if (placeImages.Any())
                    {
                        context.PlaceImages.RemoveRange(placeImages);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    // If it doesn't exist, create it now
                    Directory.CreateDirectory(uploadsFolder);
                }
    
                var baseUrl = urlProvider.GetBaseUrl();

                // 3. Upload new files (Limit to max 5)
                foreach (var file in model.StoreImages.Take(5).Where(f => f.Length > 0))
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    } // The stream is properly closed and disposed here
        
                    place.Images.Add(new PlaceImage
                    {
                        ImageUrl = $"{baseUrl}/images/stores/{place.PlaceId}/additional/{uniqueFileName}",
                        PlaceId = place.PlaceId
                    });
                }
    
                // Save the new image records to the database
                await context.SaveChangesAsync();
            }

            // --- Admin-only fields ---
            if (isAdmin)
            {
                if (model.Status.HasValue)
                    place.State = model.Status.Value;

                // Commercial Registration — Admin only
                if (!string.IsNullOrEmpty(model.CommercialRegisterNumber))
                    place.CommericalRegisteration.CrNumber = model.CommercialRegisterNumber;

                if (!string.IsNullOrEmpty(model.CompanyName))
                    place.CommericalRegisteration.EntityName = model.CompanyName;

                if (model.CommercialRegisterImage != null && model.CommercialRegisterImage.Length > 0)
                {
                    var file = model.CommercialRegisterImage;
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    string uploadsFolder = Path.Combine(env.WebRootPath, $"images/stores/{place.PlaceId}/cr");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    var baseUrl = urlProvider.GetBaseUrl();
                    place.CommericalRegisteration.ImagePath = $"{baseUrl}/images/stores/{place.PlaceId}/cr/{uniqueFileName}";
                }
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Result<EditStore>.Error("حدث خطأ أثناء تحديث بيانات المحل. يرجى التأكد من صحة البيانات.");
            }

            return Result<EditStore>.Success(model);
        }

        public async Task<Result<StoreDetailsForEditDto>> GetStoreForEdit(int placeId)
        {
            var place = context.Places.Select(x => new StoreDetailsForEditDto
            {
                PlaceId = x.PlaceId,
                Status = x.State,
                Name = x.PlaceName,
                PrimaryCategoryId = x.SubCategory.CategoryId,
                SubCategoryId = x.SubCategory.CategoryId,
                Description = x.Description,
                OpeningTime = x.OpeningTime,
                ClosingTime = x.ClosingTime,
                DirectorateId = x.Location.DirectorateId,
                DistrictId = x.Location.DistrictId,
                NearestLandmark = x.Location.NearestLandmark,
                Latitude = x.Location.Latitude,
                Longitude = x.Location.Longitude,
                CommercialRegisterNumber = x.CommericalRegisteration.CrNumber,
                CompanyName = x.CommericalRegisteration.EntityName,
                CommercialRegisterImageUrl = x.CommericalRegisteration.ImagePath,
                Phones = x.Phones.Select(r => r.Number).ToList(),
                StoreImageUrls = x.Images.Select(i => i.ImageUrl).ToList()
                
            }).FirstOrDefault(x => x.PlaceId == placeId);

            if (place == null)
            {
                return Result<StoreDetailsForEditDto>.NotFound("المحل غير موجود");
            }
            
            return Result<StoreDetailsForEditDto>.Success(place);
        }
    }
}