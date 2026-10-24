
using Application_Layer.Models.RequestDTO.PlaceController;
using Application_Layer.Models.ResponseDTO.PlaceController;
using Application_Layer.Services.PlacesManagement;

namespace WebAPI.Controllers.Customer;

[ApiController]
public class PlaceController(IPlaceService placeService) : ControllerBase
{
    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.placetoredirect)]
    public async Task<Result<PlaceDto>> PlaceRedirect(string placeName)
    {
       return await placeService.PlaceRedirect(placeName);
    }

    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.SearchPlaces)]
    public async Task<Result<ListPlaces<PlaceDto>>> SearchPlaces(string placeName,PlaceOrderBy? orderBy, string? categoryName, string? subCategoryName, string? districtName,
        string? neighborhoodName, int? openYear,double? minRating,int? minReviewCount, int pageNumber = 1, int pageSize = 10)
    {
        return await placeService.SearchPlaces(placeName,orderBy, categoryName,
            subCategoryName, districtName, neighborhoodName, openYear, minRating, minReviewCount, pageNumber, pageSize);
    }
    
    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.SearchPlaceById)]
    public async Task<Result<PlaceDto>> SearchPlaceById(int placeId)
    {
        return await placeService.SearchPlaceById(placeId);
    }

    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.GetAllPlaces)]
    public async Task<Result<ListPlaces<PlaceDto>>> GetAllPlaces(int pageNumber = 1, int pageSize = 10)
    {
        return await placeService.GetAllPlaces(pageNumber, pageSize);
    }
    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.GetPlacesOfCategory)]
    public async Task<Result<List<PlaceDto>>> GetPlacesOfCategory(string resourcename)
    {
        return await placeService.GetPlacesOfCategory(resourcename);
    }


    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.FilterPlaces)]
    public async Task<Result<ListPlaces<PlaceDto>>> FilterPlaces(PlaceOrderBy? orderBy,int pageNumber = 1, int pageSize = 10)
    {
        return await placeService.FilterPlaces(orderBy, pageNumber, pageSize);
    }
    
    
    [TranslateResultToActionResult]
    [HttpPost(Routing.CustomerPlaces.GetCategoryPlaces)]
    public async Task<Result<ListPlaces<PlaceDto>>> GetCategoryPlaces(CategoriesPageRequest model)
    {
        return await placeService.GetCategoryPlaces(model);
    }

    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.GetCategoryPlacesById)]
    public async Task<Result<ListPlaces<PlaceDto>>> GetCategoryPlacesById(int categoryId, int? subCategoryId, SortingOptions? sortingOptions, int pageNumber =  1, int pageSize = 10)
    {
        return await placeService.GetCategoryPlacesById(categoryId, subCategoryId, sortingOptions, pageNumber, pageSize);
    }

    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.GetSubCategoryPlacesById)]
    public async Task<Result<ListPlaces<PlaceDto>>> GetSubCategoryPlacesById(int subCategoryId,
        SortingOptions? sortingOptions, int pageNumber = 1, int pageSize = 10)
    {
        return await placeService.GetSubCategoryPlacesById(subCategoryId, sortingOptions, pageNumber, pageSize);
    }
    
    
    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.PlaceDetails)]
    public async Task<Result<PlaceRatesDto>> RatesAndCount(int placeId,PlaceFilter? filter, int pageNumber = 1, int pageSize = 10)
    {
        return await placeService.PlaceDetails(placeId, filter, pageNumber, pageSize);
    }
    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerPlaces.RatesAndReactions)]
    public async Task<Result<List<UserRate>>> RatesAndReactions(int placeId, int pageNumber = 1, int pageSize= 10)
    {
        return await placeService.RatesAndReactions(placeId, pageNumber, pageSize);
    }
}
