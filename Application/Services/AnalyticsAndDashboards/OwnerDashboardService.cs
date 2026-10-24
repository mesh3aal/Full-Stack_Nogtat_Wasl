using Application_Layer.Models.RequestDTO.PlaceController;
using Application_Layer.Services.InfrastructureAndUtilities;
using Microsoft.Extensions.Logging;
using Application_Layer.Models.RequestDTO.StoreController;

namespace Application_Layer.Services.AnalyticsAndDashboards
{
    public class OwnerDashboardService : IOwnerDashboardService
    {

        private readonly ApplicationDbContext _context;
        private readonly IUrlProvider _urlProvider;
        private readonly ILogger<OwnerDashboardService> _logger;
        private readonly IWebHostEnvironment _env;
        public OwnerDashboardService(ApplicationDbContext context, ILogger<OwnerDashboardService> logger, IUrlProvider urlProvider, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _urlProvider = urlProvider;
            _env = env;
        }
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/uploads/" + fileName;
        }
        public async Task<Result<StatesOwnerDto>> StatusPlace(string userID)
        {



            var totalPlace = await _context.Places.CountAsync(x => x.UserId == userID);
            var totalRate = await _context.Reviews.CountAsync(x => x.UserId == userID);

            var averageRate = totalRate == 0
                ? 0
                : await _context.Reviews.Where(x => x.UserId == userID).AverageAsync(r => r.RateValue);

            var placeStates = await _context.Places.Where(x => x.UserId == userID)
                .GroupBy(p => p.State)
                .Select(g => new
                {
                    State = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var Rejected = placeStates
                .FirstOrDefault(x => x.State == PlaceStatus.Rejected)?.Count ?? 0;

            var Pending = placeStates
                .FirstOrDefault(x => x.State == PlaceStatus.Pending)?.Count ?? 0;

            var active = placeStates
                .FirstOrDefault(x => x.State == PlaceStatus.Active)?.Count ?? 0;

            var dto = new StatesOwnerDto
            {
                totalPlase = totalPlace,
                totalReate = totalRate,
                AvergReate = averageRate,
                RejectedPlase = Rejected,
                PendingPlase = Pending,
                ActivePlase = active
            };

            return Result<StatesOwnerDto>.Success(dto, "تم جلب احصائيات");
        }
        public async Task<Result<DetailsPlaceOwnerDto>> DetailsPlaceAsync(string userid,int id)
        {
            var place = await _context.Places.Where(x=>x.UserId==userid)
                .Include(p => p.Images).Include(p => p.Rates).ThenInclude(r => r.User).Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlaceId == id);

            if (place == null)
                return Result<DetailsPlaceOwnerDto>.NotFound("المكان غير موجود");

            var totalRates = place.Rates.Count();
            var average = totalRates == 0
                                      ? 0
                                      : place.Rates.Average(x => x.RateValue);

            var dto = new DetailsPlaceOwnerDto
            {
                PLaceName = place.PlaceName,
                Status = place.State.ToString(),
                Imges = place.Images.Select(i => i.ImageUrl).ToList(),
                PlaceId = id,
                Rating = place.Rates.Select(x => new RatingDto
                {
                    TotalReviews = totalRates,
                    Average = average,
                    Distribution = new RatingDistributionDto
                    {
                        Five = place.Rates.Count(x => x.RateValue == 5),
                        Four = place.Rates.Count(x => x.RateValue == 4),
                        Three = place.Rates.Count(x => x.RateValue == 3),
                        Two = place.Rates.Count(x => x.RateValue == 2),
                        One = place.Rates.Count(x => x.RateValue == 1)
                    }
                }).FirstOrDefault(),

                LatestReviews = place.Rates
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => new ReviewDto
            {
                UserName = x.User.UserName,
                Rate = x.RateValue,
                Comment = x.Note,
                Date = x.CreatedAt
            }).ToList()
            };




            return Result<DetailsPlaceOwnerDto>.Success(dto, "تم عرض تفاصيل المكان");
        }
        public async Task<Result<PaginatedResult<ReviewDto>>> LatestReviews(string userId, int id, int Page = 1, int PageSize = 10)
        {



            var page = Page <= 0 ? 1 : Page;
            var pageSize = PageSize <= 0 ? 10 : PageSize;


            var place = await _context.Places.Where(x => x.UserId == userId)
               .Include(p => p.Images).Include(p => p.Rates).ThenInclude(r => r.User).Include(x => x.User)
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.PlaceId == id);

            var totalCount = place.Rates.Count();

            var LatestReview = place.Rates
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ReviewDto
            {
                UserName = x.User.UserName,
                Rate = x.RateValue,
                Comment = x.Note,
                Date = x.CreatedAt
            }).Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();



            var paginatedReview = PaginatedResult<ReviewDto>
                .Sucess(LatestReview, totalCount, page, pageSize);

            return Result<PaginatedResult<ReviewDto>>
                .Success(paginatedReview, "تم جلب البيانات بنجاح");


        }
        public async Task<Result<RejectedDetailsOwnerDto>> RejectedDetailsOwnerAsync(int id)
        {
            var result = await _context.Places.Include(x => x.Request)
                .Where(p => p.PlaceId == id)
                .Select(p => new
                {
                    p.State,
                    Dto = new RejectedDetailsOwnerDto
                    {

                        RejectedMessge = p.Request.RejectionReason,

                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null)
                return Result<RejectedDetailsOwnerDto>.NotFound("المكان غير موجود");

            if (result.State != PlaceStatus.Rejected)
                return Result<RejectedDetailsOwnerDto>.Conflict($"لا يمكن عرض تفاصيل الرفض لأن حالة المكان هي: {result.State}");

            return Result<RejectedDetailsOwnerDto>.Success(result.Dto, "تم عرض تفاصيل رفض المكان");
        }
        public async Task<Result<PaginatedResult<ResponsePlaseOwnerDto>>> GetPlaseAllAsync(string userid,OwnerDashbordFilter filter)
        {
            var query = _context.Places
                .AsNoTracking()
                .AsQueryable().Where(x=>x.UserId==userid);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.PlaceName, $"%{filter.Search}%"));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.State == filter.Status);
            }

            var totalCount = await query.CountAsync();

            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var places = await query
                .Select(x => new ResponsePlaseOwnerDto
                {
                    id = x.PlaceId,
                    PlaceName = x.PlaceName,
                    Status = x.State,

                    AverageRate = x.Rates.Any()
                        ? x.Rates.Average(r => r.RateValue)
                        : 0,

                    RatesCount = x.Rates.Count(),

                    imgeUrl = x.Images!
                        .OrderByDescending(i => i.PlaceImageId)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedPlace = PaginatedResult<ResponsePlaseOwnerDto>
                .Sucess(places, totalCount, page, pageSize);

            return Ardalis.Result.Result<PaginatedResult<ResponsePlaseOwnerDto>>
                .Success(paginatedPlace, "تم جلب البيانات بنجاح");
        }
        public async Task<Result<List<DirectorateDto>>> GetDirectorateAllAsync()
        {
            var directorates = await _context.Directorates
                .AsNoTracking()
                .Select(d => new DirectorateDto
                {
                    id = d.Id,
                    Name = d.Name,
                    Districts = d.Neighborhoods.Select(x => new DistrictDto
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList()
                })
                .ToListAsync();
            return Result.Success(directorates);
        }







        public async Task<Result<CreatePlaceOwnerDto>> CreatePlaceAsync(CreatePlaceOwnerDto dto, string userId)
        {
            try
            {
                if (dto == null)
                    return Result<CreatePlaceOwnerDto>.Error("بيانات غير صالحة");

                var user = await _context.Users
                    .Where(x => x.Role.Name == "Owner")
                    .FirstOrDefaultAsync(x => x.Id == userId);

                if (user == null)
                    return Result<CreatePlaceOwnerDto>.NotFound("المستخدم غير موجود");

                var commercial = await _context.Registerations
                    .FirstOrDefaultAsync(x => x.CrNumber == dto.CommercialRegisterNumber);

                if (commercial == null)
                {
                    var commercialImagePath =  await SaveFileAsync(dto.ImagePathCommercialRegister);

                    commercial = new CommericalRegisteration
                    {
                        CrNumber = dto.CommercialRegisterNumber,
                        EntityName = dto.CompanyName,
                        ExpirationDate = DateTime.UtcNow.AddYears(3),
                        UserId = user.Id,
                        ImagePath = $"{_urlProvider.GetBaseUrl()}/{commercialImagePath}"
                    };

                    _context.Registerations.Add(commercial);
                    // لا داعي لاستدعاء SaveChangesAsync هنا، EF سيهتم بالربط
                }

                var directorateExists = await _context.Directorates.AnyAsync(x => x.Id == dto.DirectorateId);
                if (!directorateExists)
                    return Result.Error("المديرية غير موجودة");

                var districtExists = await _context.Districts.AnyAsync(x => x.Id == dto.DistrictId);
                if (!districtExists)
                    return Result.Error("المنطقة غير موجودة");

                var subCategoryExists = await _context.SubCategories.AnyAsync(x => x.SubCategoryId == dto.SubCategoryId);

                if (!subCategoryExists)
                    return Result.Error("التصنيف الفرعي غير موجود");

                var request = new Request
                {
                    UserId = user.Id,
                    State = RequestStates.Pending,
                    CreatedAt = DateTime.UtcNow,

                    Place = new Place
                    {
                        UserId = user.Id,
                        PlaceName = dto.PlaceName,
                        OpeningTime = dto.OpeningTime,
                        ClosingTime = dto.ClosingTime,
                        Location = new Location
                        {
                            NearestLandmark = dto.NearestLandmark,
                            Latitude = dto.Latitude,
                            Longitude = dto.Longitude,
                            DistrictId = dto.DistrictId,
                            DirectorateId = dto.DirectorateId
                        },

                        State = PlaceStatus.Pending,
                        SubCategoryId = dto.SubCategoryId,
                        Description = dto.Description,
                        CommericalRegisteration = commercial // نربط الكائن مباشرة
                    }
                };

                var MineCategoryExists = await _context.Categories.AnyAsync(x => x.CategoryId == dto.MainCategoryId);

                if (!MineCategoryExists)
                    return Result.Error("التصنيف الريسي غير موجود");

                if (dto.Images != null && dto.Images.Any())
                {
                    foreach (var image in dto.Images)
                    {
                        var path = await SaveFileAsync(image);
                        request.Place.Images.Add(new PlaceImage { ImageUrl  = $"{_urlProvider.GetBaseUrl()}/{path}"});
                    }
                }

                if (dto.PhoneNumber != null && dto.PhoneNumber.Any())
                {
                    foreach (var Number in dto.PhoneNumber)
                    {
                        request.Place.Phones.Add(new PhoneNumber
                        {
                            Number = Number,
                        });
                    }
                }
                _context.Requests.Add(request);

                await _context.SaveChangesAsync();

                return Result<CreatePlaceOwnerDto>.Success(dto, "تم إنشاء المحل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating place for user {UserId}", userId);
                return Result<CreatePlaceOwnerDto>.Error("حدث خطأ أثناء إنشاء المحل");
            }
        }
        
        public async Task<Result<EditingForStoreDto>> UpdatePlaceAsync(int placeId, EditingForStoreDto dto, string userId)
        {
            var place = await _context.Places.Include(x => x.SubCategory).ThenInclude(p => p.Category)
                .Include(p => p.CommericalRegisteration)
                .Include(p => p.Location).ThenInclude(x => x.District).ThenInclude(d => d.Directorate)
                .Include(p => p.Images)
                .Include(p => p.Phones)
                .FirstOrDefaultAsync(p => p.PlaceId == placeId);

            if (place == null)
            {
                return Result<EditingForStoreDto>.NotFound("المحل غير موجود");
            }


            // التحقق من الملكية
            if (place.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update place {PlaceId} belonging to another user.", userId, placeId);
                return Result.Error("ليس لديك صلاحية لتعديل هذا المحل");
            }

            // التحقق من السجل التجاري
            var commercial = await _context.Registerations
                .FirstOrDefaultAsync(x => x.CrNumber == dto.CommercialRegisterNumber);

            if (commercial == null)
                return Result.NotFound("السجل التجاري غير موجود");

            // التأكد أن السجل التجاري يخص نفس المستخدم لتجنب التلاعب
            if (commercial.UserId != userId)
            {
                return Result.Error("السجل التجاري المزود لا ينتمي لحسابك");
            }



            if (place.State == PlaceStatus.Active)
            {

                // Update basic details
                if (!string.IsNullOrEmpty(dto.Name)) place.PlaceName = dto.Name;
                if (!string.IsNullOrEmpty(dto.Description)) place.Description = dto.Description;
                if (dto.SubCategoryId.HasValue && dto.SubCategoryId.Value != place.SubCategoryId)
                {
                    var subCategoryExists = await _context.SubCategories.AnyAsync(sc => sc.SubCategoryId == dto.SubCategoryId.Value);
                    if (!subCategoryExists)
                    {
                        return Result<EditingForStoreDto>.Error("التصنيف الفرعي المختار غير موجود");
                    }
                    place.SubCategoryId = dto.SubCategoryId.Value;
                }
                if (dto.OpeningTime.HasValue) place.OpeningTime = dto.OpeningTime;
                if (dto.ClosingTime.HasValue) place.ClosingTime = dto.ClosingTime;
                if (dto.Status.HasValue) place.State = dto.Status.Value;

                // Update Location
                if (place.Location == null)
                {
                    place.Location = new Domain_Layer.Entities.Location();
                }
                if (dto.DirectorateId.HasValue && dto.DirectorateId.Value != place.Location.DirectorateId)
                {
                    var exists = await _context.Directorates.AnyAsync(d => d.Id == dto.DirectorateId.Value);
                    if (!exists) return Result<EditingForStoreDto>.Error("المديرية المختارة غير موجودة");
                    place.Location.DirectorateId = dto.DirectorateId.Value;
                }

                if (dto.DistrictId.HasValue && dto.DistrictId.Value != place.Location.DistrictId)
                {
                    var exists = await _context.Districts.AnyAsync(d => d.Id == dto.DistrictId.Value);
                    if (!exists) return Result<EditingForStoreDto>.Error("المنطقة المختارة غير موجودة");
                    place.Location.DistrictId = dto.DistrictId.Value;
                }
                if (!string.IsNullOrEmpty(dto.NearestLandmark)) place.Location.NearestLandmark = dto.NearestLandmark;
                if (dto.Latitude.HasValue) place.Location.Latitude = dto.Latitude.Value;
                if (dto.Longitude.HasValue) place.Location.Longitude = dto.Longitude.Value;




                // Save Additional Images
                if (dto.StoreImages != null && dto.StoreImages.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, $"images/stores/{place.PlaceId}/additional");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var baseUrl = _urlProvider.GetBaseUrl();
                    foreach (var file in dto.StoreImages.Take(5))
                    {
                        if (file.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(file.FileName).ToLower();
                            string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                            place.Images.Add(new Domain_Layer.Entities.PlaceImage
                            {
                                ImageUrl = $"{baseUrl}/images/stores/{place.PlaceId}/additional/{uniqueFileName}",
                                PlaceId = place.PlaceId
                            });
                        }
                    }
                }

                if (dto.PhoneNumbers != null)
                {
                    place.Phones.Clear();

                    foreach (var phone in dto.PhoneNumbers)
                    {
                        if (!string.IsNullOrWhiteSpace(phone))
                        {
                            place.Phones.Add(new Domain_Layer.Entities.PhoneNumber
                            {
                                Number = phone,
                                PlaceId = place.PlaceId
                            });
                        }
                    }
                }


            }
            else if (place.State==PlaceStatus.Rejected)
            {



            }

            else
            {
                // Update Commercial Registration
                if (place.CommericalRegisteration == null)
                {
                    place.CommericalRegisteration = new Domain_Layer.Entities.CommericalRegisteration();
                }
                if (!string.IsNullOrEmpty(dto.CommercialRegisterNumber)) place.CommericalRegisteration.CrNumber = dto.CommercialRegisterNumber;
                if (!string.IsNullOrEmpty(dto.CompanyName)) place.CommericalRegisteration.EntityName = dto.CompanyName;

                // Save CR Image
                if (dto.CommercialRegisterImage != null && dto.CommercialRegisterImage.Length > 0)
                {
                    var file = dto.CommercialRegisterImage;
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    string uploadsFolder = Path.Combine(_env.WebRootPath, $"images/stores/{place.PlaceId}/cr");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    var baseUrl = _urlProvider.GetBaseUrl();
                    place.CommericalRegisteration.ImagePath = $"{baseUrl}/images/stores/{place.PlaceId}/cr/{uniqueFileName}";
                }
            }




            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Result<EditingForStoreDto>.Error("حدث خطأ أثناء تحديث بيانات المحل. يرجى التأكد من صحة البيانات.");
            }

            return Result<EditingForStoreDto>.Success(dto, "تم تحديث بيانات المحل بنجاح");
        }
        public async Task<Result<string>> DeletePlaceAsync(int id, string userId)
        {
            try
            {
                var place = await _context.Places
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.PlaceId == id);

                if (place == null)
                {
                    return Result<string>.NotFound("المكان غير موجود");
                }

                if (place.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} unauthorized attempt to delete place {PlaceId}", userId, id);
                    return Result<string>.Error("ليس لديك صلاحية لحذف هذا المكان");
                }

                var webRootPath = _env.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");

                if (place.Images != null && place.Images.Any())
                {
                    foreach (var image in place.Images)
                    {
                        if (!string.IsNullOrWhiteSpace(image.ImageUrl))
                        {
                            try
                            {
                                string relativePath = string.Empty;

                                if (image.ImageUrl.StartsWith("/uploads/"))
                                {
                                    relativePath = image.ImageUrl.TrimStart('/');
                                }
                                else if (image.ImageUrl.Contains("/images/stores/"))
                                {
                                    int startIndex = image.ImageUrl.IndexOf("/images/stores/");
                                    if (startIndex >= 0)
                                    {
                                        relativePath = image.ImageUrl.Substring(startIndex + 1);
                                    }
                                }

                                if (!string.IsNullOrEmpty(relativePath))
                                {
                                    var fullPath = System.IO.Path.Combine(webRootPath, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
                                    if (System.IO.File.Exists(fullPath))
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Could not delete image file {ImageUrl}", image.ImageUrl);
                            }
                        }
                    }
                }

                var placeDirectory = System.IO.Path.Combine(webRootPath, "images", "stores", id.ToString());
                if (System.IO.Directory.Exists(placeDirectory))
                {
                    try
                    {
                        System.IO.Directory.Delete(placeDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not delete place directory {Directory}", placeDirectory);
                    }
                }

                _context.Places.Remove(place);
                await _context.SaveChangesAsync();

                return Result<string>.Success("تم حذف المكان بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting place {PlaceId} for user {UserId}", id, userId);
                return Result<string>.Error("حدث خطأ أثناء حذف المكان");
            }
        }




    }
}
