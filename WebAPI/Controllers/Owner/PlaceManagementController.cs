using System.Security.Claims;
using Application_Layer.Models.RequestDTO.PlaceController;
using Application_Layer.Models.RequestDTO.StoreController;
using Application_Layer.Services.AnalyticsAndDashboards;

namespace WebAPI.Controllers.Owner
{

    [ApiController]
    public class PlaceManagementController : ControllerBase
    {


        private readonly IOwnerDashboardService _ownerDashboardService;

        public PlaceManagementController(IOwnerDashboardService ownerDashboardService)
        {
            _ownerDashboardService = ownerDashboardService;
        }

        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]

        [HttpGet(Routing.OwnerPlaces.GetAll)]
        public async Task<Result<PaginatedResult<ResponsePlaseOwnerDto>>> GetAllPlaces([FromQuery] OwnerDashbordFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _ownerDashboardService.GetPlaseAllAsync(userId, filter);

        }



        [TranslateResultToActionResult]
        [HttpGet(Routing.OwnerPlaces.GetDirectorates)]
        public  Task<Result<List<DirectorateDto>>> DirectorateGetAll()
        {
            return _ownerDashboardService.GetDirectorateAllAsync();
        }


        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]

        [HttpGet(Routing.OwnerPlaces.Details)]
        public async Task<Result<DetailsPlaceOwnerDto>> DetailsPlaces(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _ownerDashboardService.DetailsPlaceAsync(userId, id);
        }




        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]

        [HttpGet("RejectedDetails/{id:int}")]
        public async Task<Result<RejectedDetailsOwnerDto>> RejectedDetails(int id)
        {
            return await _ownerDashboardService.RejectedDetailsOwnerAsync(id);
        }


        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]

        [HttpPost(Routing.OwnerPlaces.Create)]
        public async Task<Result<CreatePlaceOwnerDto>> CreatePlace([FromForm] CreatePlaceOwnerDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Result<CreatePlaceOwnerDto>.Unauthorized();

            return await _ownerDashboardService.CreatePlaceAsync(dto, userId);
        }





        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]
        [HttpPut(Routing.OwnerPlaces.Update)]
        public async Task<Result<EditingForStoreDto>> UpdatePlace(int id, [FromForm] EditingForStoreDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Result<EditingForStoreDto>.Unauthorized();

            return await _ownerDashboardService.UpdatePlaceAsync(id, dto, userId);
        }


     

        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]
        [HttpDelete(Routing.OwnerPlaces.Delete)]
        public async Task<Result<string>> DeletePlace(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Result<string>.Unauthorized();

            return await _ownerDashboardService.DeletePlaceAsync(id, userId);
        }









        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.OwnerPlaces.LatestReviews)]
        public async Task<Result<PaginatedResult<ReviewDto>>> LatestReviewsAsync(int id ,int Page=1 ,int PageSize=10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

          
            return await _ownerDashboardService.LatestReviews(userId, id,Page, PageSize);
        }







    }
}
