using Application_Layer.Models.RequestDTO.PlaceController;
using Application_Layer.Models.RequestDTO.StoreController;

namespace Application_Layer.Services.AnalyticsAndDashboards
{
    public interface IOwnerDashboardService
    {
        Task<Result<string>> DeletePlaceAsync(int id, string userId);
        Task<Result<EditingForStoreDto>> UpdatePlaceAsync(int placeId, EditingForStoreDto dto, string userId);
        Task<Result<CreatePlaceOwnerDto>> CreatePlaceAsync(CreatePlaceOwnerDto dto, string userId);
        Task<Result<PaginatedResult<ResponsePlaseOwnerDto>>> GetPlaseAllAsync(string userid,OwnerDashbordFilter filter);
        Task<Result<DetailsPlaceOwnerDto>> DetailsPlaceAsync(string userid,int id);
        Task<Result<RejectedDetailsOwnerDto>> RejectedDetailsOwnerAsync(int id);
        Task<Result<List<DirectorateDto>>> GetDirectorateAllAsync();
        Task<Result<PaginatedResult<ReviewDto>>> LatestReviews(string userId, int id, int page = 1, int pageSize = 10);
        Task<Result<StatesOwnerDto>> StatusPlace(string userId);
    }
}
