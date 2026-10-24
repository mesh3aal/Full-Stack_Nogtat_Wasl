using Application_Layer.Services.AnalyticsAndDashboards;

namespace WebAPI.Controllers.Admin
{
   // [Authorize(Roles = "Admin")]
    [ApiController]

    public class AdminDashboardController : ControllerBase
    {

        private readonly ILogger<AdminDashboardController> _logger;
        private readonly IAdminDashboardService _adminDashboardService;
        public AdminDashboardController(ILogger<AdminDashboardController> logger, IAdminDashboardService adminDashboardService)
        {
            _logger = logger;
            _adminDashboardService = adminDashboardService;
        }

       [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.AdminDashboard.GetRequestStatus)]
        public async Task<Result<StatesDto>> GetOverview()
        {
            return await _adminDashboardService.OverviewStates();
        }

        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.AdminDashboard.GetLatestRequest)]
        public async Task<Result<List<LastOrderDto>>> GetLastRequest()
        {
            return await _adminDashboardService.LastRequest();
        }



    }
}
