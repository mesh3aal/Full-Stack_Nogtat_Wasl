using Application_Layer.Models.RequestDTO.PlaceController;
using Application_Layer.Models.ResponseDTO.PlaceController;
using Application_Layer.Models.ResponseDTO.ReviewController;
using Application_Layer.Models.ResponseDTO.ReviewReactionController;
using Application_Layer.Services.IdentityAndAccess;

namespace Application_Layer.Services.PlacesManagement;

public class PlaceService(ApplicationDbContext dbContext, ICurrentUserService currentUser) : IPlaceService
{
    
    public async Task<Result<PlaceDto>> PlaceRedirect(string placeName)
    {
        var pattern = $"%{placeName}%";
        var place = await dbContext.Places.Where(x => EF.Functions.Like(x.PlaceName, pattern))
        .Select(x => new PlaceDto
        {
            Id = x.PlaceId,
            Name = x.PlaceName,
            Description = x.Description,
            Email = x.Email,
            State = x.State,
            CategoryName = x.SubCategory.Category.CategoryName,
            SubCategoryName = x.SubCategory.Name,
            CreatedAt = x.CreatedAt,
            OpeningTime = x.OpeningTime,
            ClosingTime = x.ClosingTime,
            UnavailableAt = x.UnavailableAt,
            NearestLandMark = x.Location.NearestLandmark,
            Latitude = x.Location.Latitude,
            Longitude = x.Location.Longitude,
            DistrictName = x.Location.District.Name,
            ImageUrls = x.Images.Select(i => i.ImageUrl).ToList(),
            Contacts = x.Phones.Select(p => p.Number).ToList(),
            AverageRate = x.Rates.Any() ? x.Rates.Average(v => v.RateValue).ToString("F1") : "0.0",
            RateCount = x.Rates.Count.ToString()

        }).FirstOrDefaultAsync();
        if (place == null) return Result<PlaceDto>.NotFound("لم يتم إجاد المحل المطلوب ");

        return Result<PlaceDto>.Success(place);
    }
    // بحث عن محل
    public async Task<Result<ListPlaces<PlaceDto>>> SearchPlaces
    (string placeName, PlaceOrderBy? orderBy, string? categoryName, string? subCategoryName, string? districtName,
        string? directorateName, int? openYear,double? minRating,int? minReviewCount, int pageNumber = 1, int pageSize = 10)
    {
    var query = dbContext.Places
        .Where(x => x.State == PlaceStatus.Active);

    if (!string.IsNullOrWhiteSpace(categoryName))
    {
        query = query.Where(p => p.SubCategory.Category.CategoryName == categoryName);
    }
    
    if (!string.IsNullOrWhiteSpace(subCategoryName))
    {
        query = query.Where(p => p.SubCategory.Name == subCategoryName);
    }
    
    if (!string.IsNullOrWhiteSpace(directorateName))
    {
        query = query.Where(p => p.Location.Directorate.Name ==  directorateName);
    }
    
    if (!string.IsNullOrWhiteSpace(districtName))
    {
        query = query.Where(p => p.Location.District.Name == districtName);
    }
    
    if (openYear.HasValue)
    {
        query = query.Where(p => p.CreatedAt.Year == openYear);
    }
    
    if (minRating.HasValue)
    {
        query = query.Where(p => p.Rates.Average(r => r.RateValue) >= minRating);
    }
    
    if (minReviewCount.HasValue)
    {
        query = query.Where(p => p.Rates.Count >= minReviewCount);
    }
    
    if (!string.IsNullOrWhiteSpace(placeName))
    {
        var pattern = $"%{placeName}%";
        query = query.Where(x => EF.Functions.Like(x.PlaceName, pattern));
    }

    if (orderBy.HasValue)
    {
        query = orderBy switch
        {
            PlaceOrderBy.HighestRating => query.OrderByDescending(x => x.Rates.Average(x => x.RateValue)),
            PlaceOrderBy.Newest => query.OrderByDescending(x => x.CreatedAt),
            PlaceOrderBy.Recommended => query.OrderByDescending(x => ((x.Rates.Average(r => r.RateValue) * x.Rates.Count)) / (x.Rates.Count + 5)),
        _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }
    var data = query.Select(x => new PlaceDto
    {
        Id = x.PlaceId,
        Name = x.PlaceName,
        Description = x.Description,
        Email = x.Email,
        State = x.State,
        CategoryName = x.SubCategory.Category.CategoryName,
        SubCategoryName = x.SubCategory.Name,
        CreatedAt = x.CreatedAt,
        OpeningTime = x.OpeningTime,
        ClosingTime = x.ClosingTime,
        UnavailableAt = x.UnavailableAt,
        NearestLandMark = x.Location.NearestLandmark,
        Latitude = x.Location.Latitude,
        Longitude = x.Location.Longitude,
        DistrictName = x.Location.District.Name,
        ImageUrls = x.Images.Select(i => i.ImageUrl).ToList(),
        Contacts = x.Phones.Select(p => p.Number).ToList(),
        AverageRate = x.Rates.Any() ? x.Rates.Average(v => v.RateValue).ToString("F1") : "0.0",
        RateCount = x.Rates.Count.ToString()
    });
    var skip = (pageNumber - 1) * pageSize;
    var take = pageSize;
    var remaining = await data.CountAsync() - (skip + take) < 0 ? 0 : await data.CountAsync() - (skip + take);
    
    data = data.Skip(skip).Take(take);
    var listOfData = await data.AsNoTracking() .AsSplitQuery() .ToListAsync();

    ListPlaces<PlaceDto> final = new ListPlaces<PlaceDto>
    {
        Places = listOfData,
        RemainingPlacesCount = remaining
    };

    return Result<ListPlaces<PlaceDto>>.Success(final);
}
    
    public async Task<Result<PlaceDto>> SearchPlaceById(int placeId)
    {
        {
            var mainQuery = await dbContext.Places
                .Where(p => p.State == PlaceStatus.Active)
                .Select(p => new
                {
                    ID = p.PlaceId,
                    PlaceName = p.PlaceName,
                    Description = p.Description,
                    EmailAddress = p.Email,
                    CategoryName = p.SubCategory.Category.CategoryName,
                    CategoryId = p.SubCategory.Category.CategoryId,
                    subCaegoryName = p.SubCategory.Name,
                    SubCategoryId = p.SubCategoryId,
                    Status = p.State,
                    CreatedAt = p.CreatedAt,
                    OpenTime = p.OpeningTime,
                    CloseTime = p.ClosingTime,
                    UnavailableAt = p.UnavailableAt,
                    ClosestMark = p.Location.NearestLandmark,
                    Latitude = p.Location.Latitude,
                    Longtitude = p.Location.Longitude,
                    DistrictName = p.Location.District.Name,
                    Images = p.Images.Select(i => i.ImageUrl).ToList(),
                    Contacts = p.Phones.Select(c => c.Number).ToList(),
                    AverageRates = p.Rates.Average(r => r.RateValue),
                    RateCount = p.Rates.Count
                })
                .Select(x => new PlaceDto
                {
                    Id = x.ID,
                    Name = x.PlaceName,
                    Description = x.Description,
                    Email = x.EmailAddress,
                    CategoryName = x.CategoryName,
                    SubCategoryName = x.subCaegoryName,
                    State = x.Status,
                    CreatedAt = x.CreatedAt,
                    OpeningTime = x.OpenTime,
                    ClosingTime = x.CloseTime,
                    UnavailableAt = x.UnavailableAt,
                    NearestLandMark = x.ClosestMark,
                    Latitude = x.Latitude,
                    Longitude = x.Longtitude,
                    DistrictName = x.DistrictName,
                    ImageUrls = x.Images,
                    Contacts = x.Contacts,
                    AverageRate = x.AverageRates.ToString("F1"),
                    RateCount = x.RateCount.ToString()
                }).AsQueryable().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == placeId);
            if (mainQuery == null)
            {
                return Result<PlaceDto>.NotFound();
            }

            return Result<PlaceDto>.Success(mainQuery);
        }
    }

    public async Task<Result<ListPlaces<PlaceDto>>> GetAllPlaces(int pageNumber = 1, int pageSize = 10)
    {
        var places = dbContext.Places.Select(x => new PlaceDto
        {
            Id = x.PlaceId,
            Name = x.PlaceName,
            Description = x.Description,
            Email = x.Email,
            CategoryName = x.SubCategory.Category.CategoryName,
            SubCategoryName = x.SubCategory.Name,
            State = x.State,
            CreatedAt = x.CreatedAt,
            OpeningTime = x.OpeningTime,
            ClosingTime = x.ClosingTime,
            UnavailableAt = x.UnavailableAt,
            NearestLandMark = x.Location.NearestLandmark,
            Longitude = x.Location.Longitude,
            Latitude = x.Location.Latitude,
            DistrictName = x.Location.District.Name,
            ImageUrls = x.Images.Select(i => i.ImageUrl).ToList(),
            Contacts = x.Phones.Select(x => x.Number).ToList(),
            AverageRate = x.Rates.Average(x => x.RateValue).ToString("F2"),
            RateCount = x.Rates.Count.ToString()
        });
        var take = pageSize;
        var skip = (pageNumber - 1) * pageSize;
        var remaining = await places.CountAsync() - (skip + take) < 0 ? 0 : await places.CountAsync() - (skip + take);
        var data = await places.Skip(skip)
        .Take(take)
        .ToListAsync();
        ListPlaces<PlaceDto> final = new ListPlaces<PlaceDto>
        {
            Places = data,
            RemainingPlacesCount = remaining
        };
        return Result<ListPlaces<PlaceDto>>.Success(final);
    }

    // دالة تعمل عمل الـRecommendation و الاحدث و الاكثر تقيماً.
    public async Task<Result<ListPlaces<PlaceDto>>> FilterPlaces(PlaceOrderBy? orderBy, int pageNumber = 1, int pageSize = 10)
    {
        //int minReviews = 5;
        //double defaultRating = 3.0;

        // 1. الاستعلام الأساسي والفلترة
        var query = dbContext.Places
            .Where(p => p.State == PlaceStatus.Active);

        // 2. (Projection)
        var projectedQuery = query.Select(p => new
        {
            placeId = p.PlaceId,
            PlaceName = p.PlaceName,
            Description = p.Description,
            EmailAddress = p.Email,
            CategoryName = p.SubCategory.Category.CategoryName,
            CategoryId = p.SubCategory.Category.CategoryId,
            SubCategoryName = p.SubCategory.Name,
            SubCategoryId = p.SubCategory.CategoryId,
            State = p.State,
            CreatedAt = p.CreatedAt,
            OpenTime = p.OpeningTime,
            CloseTime = p.ClosingTime,
            UnavailableAt = p.UnavailableAt,
            ClosestMark = p.Location.NearestLandmark,
            Latitude = p.Location.Latitude,
            Longitude = p.Location.Longitude,
            DistrictName = p.Location.District.Name,
            Images = p.Images.Select(i => i.ImageUrl).ToList(),
            Contacts = p.Phones.Select(c => c.Number).ToList(),
            AverageRates = p.Rates.Average(r => r.RateValue),
            RateCount = p.Rates.Count
        });

        // 3. الفلترة على الحسب النوع المطلوب ( الاكثر حداثة، الاكثر تقيماً، المقترح)
        projectedQuery = orderBy switch
        {
            PlaceOrderBy.HighestRating => projectedQuery.OrderByDescending(x => x.AverageRates),
            PlaceOrderBy.Newest => projectedQuery.OrderByDescending(x => x.CreatedAt),
            PlaceOrderBy.Recommended => projectedQuery.OrderByDescending(x =>
                (x.AverageRates * x.RateCount) / (x.RateCount + x.RateCount)),
            _ => projectedQuery.OrderByDescending(x => x.CreatedAt)
        };
        var skip = (pageNumber - 1) * pageSize;
        var take = pageSize;
        var remaining = await projectedQuery.CountAsync() - (skip + take) < 0 ? 0 : await projectedQuery.CountAsync() - (skip + take);
        var result = await projectedQuery
            .Skip(skip)
            .Take(take)
            .Select(x => new PlaceDto
            {
                Id = x.placeId,
                Name = x.PlaceName,
                Description = x.Description,
                Email = x.EmailAddress,
                CategoryName = x.CategoryName,
                SubCategoryName = x.SubCategoryName,
                State = x.State,
                CreatedAt = x.CreatedAt,
                OpeningTime = x.OpenTime,
                ClosingTime = x.CloseTime,
                UnavailableAt = x.UnavailableAt,
                NearestLandMark = x.ClosestMark,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                DistrictName = x.DistrictName,
                ImageUrls = x.Images,
                Contacts = x.Contacts,
                AverageRate = x.AverageRates.ToString("F1"),
                RateCount = x.RateCount.ToString()
            }).AsQueryable().AsSplitQuery().AsNoTracking().ToListAsync();
        ListPlaces<PlaceDto> final = new ListPlaces<PlaceDto>
        {
            Places = result,
            RemainingPlacesCount = remaining
        };
        return Result<ListPlaces<PlaceDto>>.Success(final);
    }
    
    // الحصول على محلات فئة معينة بالإضافة إلى القدرة على فلترة المحلات عن طريق الفئات الخاصة + فلاتر عامة.
    public async Task<Result<ListPlaces<PlaceDto>>> GetCategoryPlaces(CategoriesPageRequest model)
    {
        var mainRequest = dbContext.Places
                // الشرط هنا ان تكون حالة المحل مفعل.
            .Where(x => x.State == PlaceStatus.Active)
            .Select(p => new
            {
                ID = p.PlaceId,
                PlaceName = p.PlaceName,
                Description = p.Description,
                EmailAddress = p.Email,
                CategoryName = p.SubCategory.Category.CategoryName,
                subCategoryName = p.SubCategory.Name,
                SubCategoryId = p.SubCategory.SubCategoryId,
                Status = p.State,
                CreatedAt = p.CreatedAt,
                OpenTime = p.OpeningTime,
                CloseTime = p.ClosingTime,
                UnavailableAt = p.UnavailableAt,
                ClosestMark = p.Location.NearestLandmark,
                Latitude = p.Location.Latitude,
                Longtitude = p.Location.Longitude,
                DistrictName = p.Location.District.Name,
                Images = p.Images.Select(i => i.ImageUrl).ToList(),
                Contacts = p.Phones.Select(c => c.Number).ToList(),
                AverageRates = p.Rates.Average(r => r.RateValue),
                RateCount = p.Rates.Count
            });
        
        if (!string.IsNullOrWhiteSpace(model.CategoryName))
        {
            mainRequest = mainRequest.Where(x => x.CategoryName.ToLower().Trim() == model.CategoryName.ToLower().Trim());
        }

        if (!string.IsNullOrWhiteSpace(model.SubCategoryName))
        {
            mainRequest = mainRequest
                .Where(x => x.subCategoryName.ToLower().Trim() == model.SubCategoryName!.ToLower().Trim());
        }
        
        var filterPlaces =  model.SortingOptions switch
        {
            SortingOptions.LowestRating => mainRequest.OrderBy(x => x.AverageRates),
            SortingOptions.MostReviewed => mainRequest.OrderByDescending(x => x.RateCount),
            SortingOptions.LeastReviewed => mainRequest.OrderBy(x => x.RateCount),
            SortingOptions.Newest => mainRequest.OrderByDescending(x => x.CreatedAt),
            SortingOptions.Earliest => mainRequest.OrderBy(x => x.CreatedAt),
            _ => mainRequest.OrderByDescending(x => x.AverageRates)
        };

        var getPlaces = filterPlaces
            .Select(x => new PlaceDto
            {
                Id = x.ID,
                Name = x.PlaceName,
                Description = x.Description,
                Email = x.EmailAddress,
                CategoryName = x.CategoryName,
                SubCategoryName = x.subCategoryName,
                State = x.Status,
                CreatedAt = x.CreatedAt,
                OpeningTime = x.OpenTime,
                ClosingTime = x.CloseTime,
                UnavailableAt = x.UnavailableAt,
                NearestLandMark = x.ClosestMark,
                Latitude = x.Latitude,
                Longitude = x.Longtitude,
                DistrictName = x.DistrictName,
                ImageUrls = x.Images,
                Contacts = x.Contacts,
                AverageRate = x.AverageRates.ToString("F1"),
                RateCount = x.RateCount.ToString()
            }).AsNoTracking();
        var skip = (model.PageNumber - 1) * model.PageNumber;
        var take = model.PageSize;
        var remaining = await getPlaces.CountAsync() - (skip + take) < 0 ? 0 : await getPlaces.CountAsync() - (skip + take);
            var data = await getPlaces
            .Skip(skip).Take(take)
            .AsSplitQuery().ToListAsync();
            ListPlaces<PlaceDto> final = new ListPlaces<PlaceDto>
            {
                Places = data,
                RemainingPlacesCount = remaining
            };
        return  Result<ListPlaces<PlaceDto>>.Success(final);
    }

    public async Task<Result<List<PlaceDto>>> GetPlacesOfCategory(string placeName)
    {
        var places = await dbContext.Places
            .Where(x=> x.SubCategory.Category.Source.EndsWith(placeName))
            .Select(p => new PlaceDto
            {
                Id = p.PlaceId,
                Name = p.PlaceName,
                Description = p.Description,
                Email = p.Email,
                CategoryName = p.SubCategory.Category.CategoryName,
                SubCategoryName = p.SubCategory.Name,
                State = p.State,
                CreatedAt = p.CreatedAt,
                OpeningTime = p.OpeningTime,
                ClosingTime = p.ClosingTime,
                UnavailableAt = p.UnavailableAt,
                Latitude = p.Location.Latitude,
                Longitude = p.Location.Longitude,
                DistrictName = p.Location.District.Name,
                ImageUrls = p.Images.Select(i => i.ImageUrl).ToList(),
                Contacts = p.Phones.Select(c => c.Number).ToList(),
                AverageRate = p.Rates.Average(r => r.RateValue).ToString(),
                RateCount = p.Rates.Count.ToString(),
            }).ToListAsync();
        return Result<List<PlaceDto>>.Success(places);
    }

    public async Task<Result<ListPlaces<PlaceDto>>> GetCategoryPlacesById(int categoryId, int? subCategoryId, SortingOptions? sortingOptions, int pageNumber =  1, int pageSize = 10)
    {
        var mainRequest = dbContext.Places
                // الشرط هنا ان تكون حالة المحل مفعل.
            .Where(x => x.State == PlaceStatus.Active)
            .Where(x => x.SubCategory.Category.CategoryId == categoryId)
            .Select(p => new
            {
                ID = p.PlaceId,
                PlaceName = p.PlaceName,
                Description = p.Description,
                EmailAddress = p.Email,
                CategoryName = p.SubCategory.Category.CategoryName,
                CategoryId = p.SubCategory.Category.CategoryId,
                SubCategoryName = p.SubCategory.Name,
                subCategoryId = p.SubCategory.SubCategoryId,
                Status = p.State,
                CreatedAt = p.CreatedAt,
                OpenTime = p.OpeningTime,
                CloseTime = p.ClosingTime,
                UnavailableAt = p.UnavailableAt,
                ClosestMark = p.Location.NearestLandmark,
                Latitude = p.Location.Latitude,
                Longtitude = p.Location.Longitude,
                DistrictName = p.Location.District.Name,
                Images = p.Images.Select(i => i.ImageUrl).ToList(),
                Contacts = p.Phones.Select(c => c.Number).ToList(),
                AverageRates = p.Rates.Average(r => r.RateValue),
                RateCount = p.Rates.Count
            });
        if (subCategoryId != null)
        {
            mainRequest = mainRequest.Where(x => x.subCategoryId == subCategoryId);
        }
        
        var filterPlaces =  sortingOptions switch
        {
            SortingOptions.LowestRating => mainRequest.OrderBy(x => x.AverageRates),
            SortingOptions.MostReviewed => mainRequest.OrderByDescending(x => x.RateCount),
            SortingOptions.LeastReviewed => mainRequest.OrderBy(x => x.RateCount),
            SortingOptions.Newest => mainRequest.OrderByDescending(x => x.CreatedAt),
            SortingOptions.Earliest => mainRequest.OrderBy(x => x.CreatedAt),
            _ => mainRequest.OrderByDescending(x => x.AverageRates)
        };

        var getPlaces = filterPlaces
            .Select(x => new PlaceDto
            {
                Id = x.ID,
                Name = x.PlaceName,
                Description = x.Description,
                Email = x.EmailAddress,
                CategoryName = x.CategoryName,
                SubCategoryName = x.SubCategoryName,
                State = x.Status,
                CreatedAt = x.CreatedAt,
                OpeningTime = x.OpenTime,
                ClosingTime = x.CloseTime,
                UnavailableAt = x.UnavailableAt,
                NearestLandMark = x.ClosestMark,
                Latitude = x.Latitude,
                Longitude = x.Longtitude,
                DistrictName = x.DistrictName,
                ImageUrls = x.Images,
                Contacts = x.Contacts,
                AverageRate = x.AverageRates.ToString("F1"),
                RateCount = x.RateCount.ToString()
            }).AsNoTracking();
            var skip = (pageNumber -1) * pageNumber;
            var take = pageSize;
            var remaining = await getPlaces.CountAsync() - (skip + take) < 0 ? 0 : await getPlaces.CountAsync() - (skip + take);
            var data = await getPlaces.Skip(skip).Take(take).AsSplitQuery().ToListAsync();

            ListPlaces<PlaceDto> final = new ListPlaces<PlaceDto>
            {
                Places = data,
                RemainingPlacesCount = remaining
            };
        return  Result<ListPlaces<PlaceDto>>.Success(final);
    }

    public async Task<Result<ListPlaces<PlaceDto>>> GetSubCategoryPlacesById(int subCategoryId, SortingOptions? sortingOptions, int pageNumber =  1, int pageSize = 10)
    {
        var mainRequest = dbContext.Places
                // الشرط هنا ان تكون حالة المحل مفعل.
            .Where(x => x.State == PlaceStatus.Active)
            .Where(x => x.SubCategory.SubCategoryId == subCategoryId)
            .Select(p => new
            {
                ID = p.PlaceId,
                PlaceName = p.PlaceName,
                Description = p.Description,
                EmailAddress = p.Email,
                CategoryName = p.SubCategory.Category.CategoryName,
                CategoryId = p.SubCategory.Category.CategoryId,
                SubCategoryName = p.SubCategory.Name,
                subCaegoryId = p.SubCategory.SubCategoryId,
                Status = p.State,
                CreatedAt = p.CreatedAt,
                OpenTime = p.OpeningTime,
                CloseTime = p.ClosingTime,
                UnavailableAt = p.UnavailableAt,
                ClosestMark = p.Location.NearestLandmark,
                Latitude = p.Location.Latitude,
                Longtitude = p.Location.Longitude,
                DistrictName = p.Location.District.Name,
                Images = p.Images.Select(i => i.ImageUrl).ToList(),
                Contacts = p.Phones.Select(c => c.Number).ToList(),
                AverageRates = p.Rates.Average(r => r.RateValue),
                RateCount = p.Rates.Count
            });
        var filterPlaces =  sortingOptions switch
        {
            SortingOptions.LowestRating => mainRequest.OrderBy(x => x.AverageRates),
            SortingOptions.MostReviewed => mainRequest.OrderByDescending(x => x.RateCount),
            SortingOptions.LeastReviewed => mainRequest.OrderBy(x => x.RateCount),
            SortingOptions.Newest => mainRequest.OrderByDescending(x => x.CreatedAt),
            SortingOptions.Earliest => mainRequest.OrderBy(x => x.CreatedAt),
            _ => mainRequest.OrderByDescending(x => x.AverageRates)
        };
        var getPlaces = filterPlaces
            .Select(x => new PlaceDto
            {
                Id = x.ID,
                Name = x.PlaceName,
                Description = x.Description,
                Email = x.EmailAddress,
                CategoryName = x.CategoryName,
                SubCategoryName = x.SubCategoryName,
                State = x.Status,
                CreatedAt = x.CreatedAt,
                OpeningTime = x.OpenTime,
                ClosingTime = x.CloseTime,
                UnavailableAt = x.UnavailableAt,
                NearestLandMark = x.ClosestMark,
                Latitude = x.Latitude,
                Longitude = x.Longtitude,
                DistrictName = x.DistrictName,
                ImageUrls = x.Images,
                Contacts = x.Contacts,
                AverageRate = x.AverageRates.ToString("F1"),
                RateCount = x.RateCount.ToString()
            }).AsNoTracking();
            var skip = (pageNumber -1) * pageNumber;
                var take = pageSize;
                var remaining = await getPlaces.CountAsync() - (skip + take) < 0 ? 0 : await getPlaces.CountAsync() - (skip + take);
                var data = await getPlaces
            .Skip(skip).Take(take).AsSplitQuery().ToListAsync();

                ListPlaces<PlaceDto> final = new ListPlaces<PlaceDto>
                {
                    Places = data,
                    RemainingPlacesCount = remaining,
                };
        
        return  Result<ListPlaces<PlaceDto>>.Success(final);
    }

    public async Task<Result<PlaceRatesDto>> PlaceDetails(int placeId, PlaceFilter? filter, int pageNumber = 1, int pageSize = 10)
    {
        var userId = currentUser.GetUserId();
        // حساب عدد كل نجمة
        var ratesData = await dbContext.Places
            .Where(r => r.PlaceId == placeId)
            .Select(x => new OneToFiveRating
            {
                FiveStar = x.Rates.Count(r => r.RateValue == 5),
                FourStar = x.Rates.Count(r => r.RateValue == 4),
                ThreeStar = x.Rates.Count(r => r.RateValue == 3),
                TwoStar = x.Rates.Count(r => r.RateValue == 2),
                OneStar = x.Rates.Count(r => r.RateValue == 1),
            }).FirstOrDefaultAsync();

        if (ratesData == null) return Result<PlaceRatesDto>.NotFound();

        
        // الحصول على التقييمات + معلومات كل مستخدم
        var rates = dbContext.Reviews
            .Include(x => x.User)
            .Where(r => r.PlaceId == placeId && !string.IsNullOrWhiteSpace(r.Note));
        
        // فلتر الملاحظات
        rates = filter switch
        {
            PlaceFilter.HighestRating => rates.OrderByDescending(r => r.RateValue),
            PlaceFilter.LowestRating => rates.OrderBy(r => r.RateValue),
            PlaceFilter.Earliest => rates.OrderBy(r => r.CreatedAt),
            PlaceFilter.Newest => rates.OrderByDescending(r => r.CreatedAt),
            _ => rates.OrderByDescending(r => r.CreatedAt)
        };
        var skip = ((pageNumber - 1) * pageSize);
        var take =  pageSize;
        var remaining = await rates.CountAsync() - (skip + take) < 0 ? 0 : await rates.CountAsync() - (skip + take);
        
        var listOfRates = await rates
            .Skip(skip)
            .Take(take)
            .Select(x => new ReviewResponse
            {
                ReviewId = x.ReviewId,
                PlaceName = x.Place.PlaceName,
                Note = x.Note,
                RateValue = x.RateValue,
                CreatedAt = x.CreatedAt,
                User = new UserInfoRate
                {
                    UserName = x.User.UserName!,
                    UserAvatar = x.User.Avatar!,
                },
                ReactionType = userId == null ? ReviewReactionStatus.NotExists
                    : x.ReviewReactions.Any(r => r.UserId == userId && r.Type == ReactionType.Like) ? ReviewReactionStatus.Liked
                    : x.ReviewReactions.Any(r => r.UserId == userId && r.Type == ReactionType.Dislike) ? ReviewReactionStatus.Disliked
                    : ReviewReactionStatus.NotExists, 
                ReactionCount = new ReviewReactionCount
                {
                    Likes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Like).Count(),
                    Dislikes = x.ReviewReactions.Where(x=> x.Type == ReactionType.Dislike).Count()
                }
            })
            .ToListAsync();
        var toListUserRate = new ListReviews<ReviewResponse>
        {
            Reviews = listOfRates,
            RemainingReviewsCount = remaining,
        };
        var placeDetail = await dbContext.Places.Where(x => x.PlaceId == placeId).Select(x => new PlaceDto
        {
            Id = x.PlaceId,
            Name = x.PlaceName,
            Description = x.Description,
            Email = x.Email,
            CategoryName = x.SubCategory.Category.CategoryName,
            SubCategoryName = x.SubCategory.Name,
            State = x.State,
            CreatedAt = x.CreatedAt,
            OpeningTime = x.OpeningTime,
            ClosingTime = x.ClosingTime,
            UnavailableAt = x.UnavailableAt,
            NearestLandMark = x.Location.NearestLandmark,
            Longitude = x.Location.Longitude,
            Latitude = x.Location.Latitude,
            DistrictName = x.Location.District.Name,
            DirectorateName = x.Location.Directorate.Name,
            ImageUrls = x.Images.Select(x => x.ImageUrl).ToList(),
            Contacts = x.Phones.Select(x => x.Number).ToList(),
            AverageRate = x.Rates.Any() ? x.Rates.Average(x=> x.RateValue).ToString("F1") : "0.0",
            RateCount = x.Rates.Count.ToString(),
        }).FirstAsync();
        // تجميع كل البيانات السابقة
        var finalData = new PlaceRatesDto
        {
            PlaceDetail = placeDetail,
            OneToFiveRating = ratesData,
            Rates = toListUserRate,
        };
        
        return Result<PlaceRatesDto>.Success(finalData);
    }

    public async Task<Result<List<UserRate>>> RatesAndReactions(int placeId, int pageNumber, int pageSize)
    {
        var placeRates = dbContext.Reviews
            .Include(x => x.User)
            .Where(r => r.PlaceId == placeId && !string.IsNullOrWhiteSpace(r.Note))
            .Select(p => new UserRate
            {
                UserName = p.User.UserName!,
                UserImage = p.User.Avatar!,
                CreatedAt = p.CreatedAt,
                Rate = p.RateValue,
                Note = p.Note,
                Likes = p.ReviewReactions.Count(x => x.Type == ReactionType.Like),
                Dislikes = p.ReviewReactions.Count(x => x.Type == ReactionType.Dislike),
            }).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return Result<List<UserRate>>.Success(await placeRates.ToListAsync());
    }
}
