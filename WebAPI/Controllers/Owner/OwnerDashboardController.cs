
using System.Security.Claims;
using Application_Layer.Services.AnalyticsAndDashboards;

namespace WebAPI.Controllers.Owner
{
    [ApiController]

    public class OwnerDashboardController : ControllerBase
    {
        private readonly IOwnerDashboardService _ownerDashboardService;

        public OwnerDashboardController(IOwnerDashboardService ownerDashboardService)
        {
            _ownerDashboardService = ownerDashboardService;
        }
        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.OwnerDashboard.Statistics)]
        public async Task<Result<StatesOwnerDto>> GetStatistics()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _ownerDashboardService.StatusPlace(userId);

        }
        [Authorize(Roles = "Owner")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.OwnerDashboard.GetAlll)]
        public async Task<Result<PaginatedResult<ResponsePlaseOwnerDto>>> GetAllPlaces([FromQuery] OwnerDashbordFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _ownerDashboardService.GetPlaseAllAsync(userId, filter);

        }
        
    }
}
