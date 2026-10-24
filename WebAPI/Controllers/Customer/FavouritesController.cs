using Application_Layer.Models.ResponseDTO.FavouriteController;
using Application_Layer.Models.ResponseDTO.PlaceController;
using Application_Layer.Services.InteractionsAndFeedback;

namespace WebAPI.Controllers.Customer;

[ApiController]
public class FavouritesController(IFavouriteService favouriteService) : ControllerBase
{
    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerFavourites.GetAll)]
    [Authorize]
    public async Task<Result<List<UserFavourites>>> GetFavourites(string? searchString, int pageNumber = 1,
        int pageSize = 10)
    {
        return await favouriteService.GetFavourites(searchString, pageNumber,pageSize);
    }
    [TranslateResultToActionResult]
    [HttpPost(Routing.CustomerFavourites.Add)]
    public async Task<Result> AddToFavourite(int placeId)
    {
        return await favouriteService.AddToFavourite(placeId);
    }
    
    [TranslateResultToActionResult]
    [HttpDelete(Routing.CustomerFavourites.Remove)]
    public async Task<Result<string>> RemoveFromFavourite(int placeId)
    {
        return await favouriteService.RemoveFromFavourite(placeId);
    }

    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerFavourites.IsFavourited)]
    public async Task<Result<bool>> IsFavourited(int placeId)
    {
        return await favouriteService.isFavourited(placeId);
    }
}
