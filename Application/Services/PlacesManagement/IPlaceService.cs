using Application_Layer.Models.RequestDTO.PlaceController;
using Application_Layer.Models.ResponseDTO.PlaceController;

namespace Application_Layer.Services.PlacesManagement;

public interface IPlaceService
{
    Task<Result<PlaceDto>> PlaceRedirect(string placeName);
    Task<Result<ListPlaces<PlaceDto>>> SearchPlaces(string placeName,PlaceOrderBy? orderBy, string? categoryName, string? subCategoryName,
        string? districtName,  string? directorateName, int? openYear, double? minRating, int? minReviewCount, int pageNumber, int pageSize);
    
    Task<Result<PlaceDto>> SearchPlaceById(int placeId);
    Task<Result<ListPlaces<PlaceDto>>> GetAllPlaces(int pageNumber = 1, int pageSize = 10);
    
    Task<Result<ListPlaces<PlaceDto>>> FilterPlaces(PlaceOrderBy? orderBy, int pageNumber = 1, int pageSize = 10);
    Task<Result<List<PlaceDto>>> GetPlacesOfCategory(string placeName);
    Task<Result<ListPlaces<PlaceDto>>> GetCategoryPlaces(CategoriesPageRequest model);
    Task<Result<ListPlaces<PlaceDto>>> GetCategoryPlacesById(int categoryId, int? subCategoryId, SortingOptions? sortingOptions, int pageNumber = 1, int pageSize = 10);
    Task<Result<ListPlaces<PlaceDto>>> GetSubCategoryPlacesById(int subCategoryId, SortingOptions? sortingOptions,
        int pageNumber = 1, int pageSize = 10);
    
    Task<Result<PlaceRatesDto>> PlaceDetails(int placeId, PlaceFilter? filter, int pageNumber, int pageSize);
    Task<Result<List<UserRate>>> RatesAndReactions(int placeId, int pageNumber, int pageSize);
}
