
using Application_Layer.Models.ResponseDTO.FavouriteController;
using Application_Layer.Services.IdentityAndAccess;

namespace Application_Layer.Services.InteractionsAndFeedback;

public class FavouriteService(ApplicationDbContext context, ICurrentUserService currentUser) :IFavouriteService
{
    public async Task<Result<List<UserFavourites>>> GetFavourites(string? searchString, int pageNumber=1,
        int pageSize=10)
    {
        var user = await currentUser.GetUserAsync();
        if (user.Status == UserStatus.PendingDeletion)
        {
            return Result<List<UserFavourites>>.Forbidden("الحساب معطل قم بإعدة التفعيل حتى ترى مفضلتك");
        }

        var places = context.Favourites
            .Include(x => x.User)
            .ThenInclude(x => x.Places)
            .ThenInclude(x => x.SubCategory)
            .Where(x => x.UserId == user.Id);

            if (!string.IsNullOrEmpty(searchString))
            {
                var pattern = $"%{searchString}%";
                places = places.Where(x => EF.Functions.Like(x.Place.PlaceName, pattern));
            }
            var orderedData = places.OrderByDescending(x =>x.FavouritedAt)
                .Select(x => new UserFavourites
            {
                PlaceId = x.PlaceId,
                PlaceName = x.Place.PlaceName,
                mainCategoryName = x.Place.SubCategory.Category.CategoryName,
                subcategoryName = x.Place.SubCategory.Name,
                Image = x.Place.Images.First().ImageUrl ?? "",
                AverageRating = x.Place.Rates.Average(r => r.RateValue).ToString("F1"),
                directorateName = x.Place.Location.Directorate.Name,
                districtName = x.Place.Location.District.Name,
                FavouritedAt = x.FavouritedAt,
            });
        var skip = ((pageNumber - 1) * pageSize);
        var take =  pageSize;
        var data = await orderedData.Skip(skip).Take(take).AsNoTracking().ToListAsync();

        
        
        return Result<List<UserFavourites>>.Success(data);
    }

    public async Task<Result> AddToFavourite(int placeId)
    {
        var user = await currentUser.GetUserAsync();
        if (user == null) return Result.Unauthorized();
    
        var placeExists = await context.Places.AnyAsync(x => x.PlaceId == placeId);
        if (!placeExists) 
            return Result.NotFound("المحل المطلوب غير موجود");

        var isAlreadyFavourite = await context.Favourites
            .AnyAsync(x => x.UserId == user.Id && x.PlaceId == placeId);

        if (isAlreadyFavourite) 
            return Result.Conflict("لقد أضفت هذا المحل من قبل");

        var newFavourite = new Favourite
        {
            PlaceId = placeId,
            UserId = user.Id,
            FavouritedAt = DateTime.UtcNow
        };
    
        context.Favourites.Add(newFavourite);
    
        if (await context.SaveChangesAsync() > 0)
        {
            return Result.Success(); 
        }

        return Result.Error("حدث خطاء بالحفظ لم نتمكن من إضافة المحل إلى مفضلتك");
    }

    public async Task<Result<string>> RemoveFromFavourite(int placeId)
    {
        var user = await currentUser.GetUserAsync();
        if (user == null) return Result<string>.Unauthorized();
        
        var place = await context.Favourites.Where(x => x.PlaceId == placeId)
            .FirstOrDefaultAsync(x => x.UserId == user.Id);
        
        if (place == null) 
            return Result.NotFound("المحل المطلوب غير موجود");
        
        context.Favourites.Remove(place);
        
        if (await context.SaveChangesAsync() <= 0)
            return Result<string>.Error("حدث خطاء بالحذف لم نتمكن من حذف المحل من مفضلتك");
        return Result.Success();
    }

    public async Task<Result<bool>> isFavourited(int placeId)
    {
        var user = await currentUser.GetUserAsync();
        if (user == null) return Result<bool>.Unauthorized();
        
        var placeExists = await context.Places.AnyAsync(x => x.PlaceId == placeId);
        if (!placeExists) 
            return Result.NotFound("المحل المطلوب غير موجود");
        
        var isFavourite = await context.Favourites.AnyAsync(x => x.UserId == user.Id && x.PlaceId == placeId);
        return Result<bool>.Success(isFavourite);
    }
}
