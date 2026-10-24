using Application_Layer.Models.ResponseDTO.FavouriteController;
using Application_Layer.Models.ResponseDTO.PlaceController;

namespace Application_Layer.Services.InteractionsAndFeedback;

public interface IFavouriteService
{
    Task<Result<List<UserFavourites>>> GetFavourites(string? searchString, int pageNumber = 1,
        int pageSize = 10);
    Task<Result> AddToFavourite(int placeId);
    Task<Result<string>> RemoveFromFavourite(int placeId);
    Task<Result<bool>> isFavourited(int placeId);
}
